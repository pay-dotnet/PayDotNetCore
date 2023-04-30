using Microsoft.Extensions.Options;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

public class BillableManager : IBillableManager
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentMethodManager _paymentMethodManager;
    private readonly IOptions<PayDotNetConfiguration> _options;

    public BillableManager(
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentMethodManager paymentMethodManager,
        IOptions<PayDotNetConfiguration> options)
    {
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _paymentMethodManager = paymentMethodManager;
        _options = options;
    }

    /// <remarks>
    /// https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
    /// </remarks>
    public async Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName, bool allowFake = false)
    {
        if (processorName.Equals(PaymentProcessors.Fake, StringComparison.OrdinalIgnoreCase) && !allowFake)
        {
            throw new PayDotNetException($"Processor `{processorName}` is not allowed");
        }

        return await _customerManager.GetOrCreateCustomerAsync(email, processorName);
    }

    public async Task<PaySubscription> SubscribeAsync(PayCustomer payCustomer, string priceId, string name)
    {
        if (_paymentMethodManager.IsPaymentMethodRequired())
        {
            PayPaymentMethod? paymentMethod = payCustomer.PaymentMethods.FirstOrDefault(p => p.IsDefault);
            if (paymentMethod == null)
            {
                throw new PayDotNetException("Customer has no default payment method");
            }
        }

        PaySubscriptionResult result = await _subscriptionManager.CreateSubscriptionAsync(payCustomer, priceId, name);
        //if (result.Payment is not null &&
        //    _options.Value.Stripe.PaymentBehaviour != PayDotNetStripeConfiguration.DefaultPaymentBehaviour &&
        //    result.PaySubscription.IsIncomplete())
        if (result.PaySubscription.IsIncomplete())
        {
            result.Payment.Validate();
        }

        return result.PaySubscription;
    }

    public Task AddPaymentMethodAsync(PayCustomer payCustomer, string paymentMethodId, bool isDefault)
    {
        return Task.CompletedTask;
    }

    public Task SavePaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethod paymentMethod, bool isDefault)
    {
        return Task.CompletedTask;
    }

    // https://stripe.com/docs/api/checkout/sessions/create
    //
    // checkout(mode: "payment")
    // checkout(mode: "setup")
    // checkout(mode: "subscription")
    //
    // checkout(line_items: "price_12345", quantity: 2)
    // checkout(line_items: [{ price: "price_123" }, { price: "price_456" }])
    // checkout(line_items: "price_12345", allow_promotion_codes: true)
    //
    public Task CheckoutAsync(PayCustomer payCustomer)
    {
        return Task.CompletedTask;
    }

    // https://stripe.com/docs/api/checkout/sessions/create
    //
    // checkout_charge(amount: 15_00, name: "T-shirt", quantity: 2)
    //
    public Task CheckoutChargeAsync(PayCustomer payCustomer)
    {
        return Task.CompletedTask;
    }

    public Task AuthorizeAsync(PayCustomer payCustomer)
    {
        return Task.CompletedTask;
    }
}