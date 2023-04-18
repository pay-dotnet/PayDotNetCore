using LanguageExt.Common;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class BillableManager : IBillableManager
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentMethodManager _paymentMethodManager;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public BillableManager(
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentMethodManager paymentMethodManager,
        IPaymentProcessorService paymentProcessorService
        )
    {
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _paymentMethodManager = paymentMethodManager;
        _paymentProcessorService = paymentProcessorService;
    }

    // https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
    public async Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName, bool allowFake = false)
    {
        if (processorName.Equals(PaymentProcessors.Fake, StringComparison.OrdinalIgnoreCase) && !allowFake)
        {
            throw new PayDotNetException($"Processor `{processorName}` is not allowed");
        }

        return await _customerManager.GetOrCreateCustomerAsync(email, processorName);
    }

    public async Task<PayCustomer> InitializeCustomerAsync(PayCustomer payCustomer, string? paymentMethodId)
    {
        PaymentProcessorCustomer customer;
        if (payCustomer.HasProcessorId())
        {
            customer = await _paymentProcessorService.FindCustomerAsync(payCustomer.Email);
        }
        else
        {
            customer = await _paymentProcessorService.CreateCustomerAsync(payCustomer.Email, new());
            payCustomer.ProcessorId = customer.Id;
            await _customerManager.UpdateAsync(payCustomer);
        }

        if (!string.IsNullOrEmpty(paymentMethodId))
        {
            PayPaymentMethod _ = await _paymentMethodManager.AddPaymentMethodAsync(customer, paymentMethodId, isDefault: true);
        }

        // Reload payCustomer due to new data being added.
        return (await _customerManager.FindByEmailAsync(payCustomer.Email, payCustomer.Processor))!;
    }

    public async Task<Result<PaySubscription>> SubscribeAsync(PayCustomer customer, string name = "default", string price = "default")
    {
        PayPaymentMethod? paymentMethod = customer.PaymentMethods.FirstOrDefault(p => p.IsDefault);
        if (paymentMethod == null)
        {
            return new(new PayDotNetException("Customer has no default payment method"));
        }

        // TODO: Standardize the trial period options
        //# Standardize the trial period options
        //if (trial_period_days = options.delete(:trial_period_days)) && trial_period_days > 0
        //  options.merge!(trial_period: true, trial_duration: trial_period_days, trial_duration_unit: :day)
        //end

        // TODO: payCustomer.Id could also be Processor.Id
        PaymentProcessorSubscription paymentProcessorSubscription = await _paymentProcessorService.CreateSubscriptionAsync(customer, price, new());
        // BrainTree vs Stripe logic is different

        PaySubscription subscription =
            await _subscriptionManager.SynchroniseAsync(paymentProcessorSubscription.Id, paymentProcessorSubscription, name);

        // No trial, payment method requires SCA
        if (subscription.Status == PayStatus.Incomplete)
        {
            // TODO: remove exception flow.
            paymentProcessorSubscription.Payment.Validate();
        }

        return subscription;
    }
}