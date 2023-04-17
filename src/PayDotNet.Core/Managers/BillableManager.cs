using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Infrastructure;

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
    public Task<PayCustomer> SetPaymentProcessorAsync(string id, string email, string processorName, bool allowFake = false)
    {
        if (processorName == PaymentProcessors.Fake && !allowFake)
        {
            throw new PayDotNetException($"Processor `{processorName}` is not allowed");
        }

        return _customerManager.CreateIfNotExistsAsync(id, email, processorName);
    }

    public async Task<PayCustomer> InitializeCustomerAsync(PayCustomer customer, string? paymentMethodId)
    {
        if (!customer.HasProcessorId())
        {
            // Create customer
            PaymentProcessorCustomer result = await _paymentProcessorService.CreateCustomerAsync(customer.Email, new());

            // Update processorId
            customer.ProcessorId = result.Id;
            await _customerManager.UpdateAsync(customer);
        }

        if (!string.IsNullOrEmpty(paymentMethodId))
        {
            PayPaymentMethod _ = await _paymentMethodManager.CreateAsync(customer, paymentMethodId, isDefault: true);
        }

        // Reload customer due to new data being reloaded.
        return (await _customerManager.FindByIdAsync(customer.Id, customer.Processor))!;
    }

    public Task<PayCustomer> ResolveCustomerAsync(PayCustomer customer)
    {
        throw new NotImplementedException();
    }

    public async Task<PaySubscription> SubscribeAsync(PayCustomer customer, string name = "default", string plan = "default")
    {
        PayPaymentMethod? paymentMethod = customer.PaymentMethods.FirstOrDefault(p => p.IsDefault);
        if (paymentMethod == null)
        {
            throw new PayDotNetException("Customer has no default payment method");
        }

        // TODO: Standardize the trial period options
        //# Standardize the trial period options
        //if (trial_period_days = options.delete(:trial_period_days)) && trial_period_days > 0
        //  options.merge!(trial_period: true, trial_duration: trial_period_days, trial_duration_unit: :day)
        //end

        // TODO: customer.Id could also be Processor.Id
        PaymentProcessorSubscription paymentProcessorSubscription = await _paymentProcessorService.CreateSubscriptionAsync(customer, plan, new());
        // BrainTree vs Stripe logic is different

        PaySubscription subscription =
            await _subscriptionManager.SynchroniseAsync(paymentProcessorSubscription.Id, paymentProcessorSubscription, name);

        // No trial, payment method requires SCA
        if (subscription.Status == PayStatus.Incomplete)
        {
            paymentProcessorSubscription.Payment.Validate();
        }

        return subscription;
    }

    public Task<PayCustomer> SetPaymentProcessorAsync(string customerId, string processorName)
    {
        throw new NotImplementedException();
    }
}
