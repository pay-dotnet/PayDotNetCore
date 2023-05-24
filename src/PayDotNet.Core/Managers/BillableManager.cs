using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Managers;

/// <summary>
/// Implementation of the facade containing the primary operations for Pay
/// </summary>
public class BillableManager : IBillableManager
{
    private readonly IChargeManager _chargeManager;
    private readonly ICheckoutManager _checkoutManager;
    private readonly ICustomerManager _customerManager;
    private readonly IPayCustomerEmailProvider _payCustomerEmailProvider;
    private readonly IPaymentMethodManager _paymentMethodManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public BillableManager(
        IPayCustomerEmailProvider payCustomerEmailProvider,
        ICheckoutManager checkoutManager,
        IChargeManager chargeManager,
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentMethodManager paymentMethodManager)
    {
        _payCustomerEmailProvider = payCustomerEmailProvider;
        _checkoutManager = checkoutManager;
        _chargeManager = chargeManager;
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _paymentMethodManager = paymentMethodManager;
    }

    public virtual Task AuthorizeAsync(PayCustomer payCustomer, PayAuthorizeChargeOptions options)
    {
        return ChargeAsync(payCustomer, options);
    }

    /// <inheritdoc/>
    public virtual async Task<IPayment> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        if (!string.IsNullOrEmpty(options.PaymentMethodId))
        {
            // Make sure the provided payment method becomes the new default.
            await _paymentMethodManager.AddPaymentMethodAsync(payCustomer, new(options.PaymentMethodId, IsDefault: true));
        }

        PayChargeResult result = await _chargeManager.ChargeAsync(payCustomer, options);

        // Let caller decide how to handle flow.
        return result.Payment;
    }

    public virtual Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        return _checkoutManager.CheckoutAsync(payCustomer, options);
    }

    public virtual Task<PayCheckoutResult> CheckoutChargeAsync(PayCustomer payCustomer, PayCheckoutChargeOptions options)
    {
        return CheckoutAsync(payCustomer, new()
        {
            AllowPromotionCodes = options.AllowPromotionCodes,
            Mode = options.Mode,
            LineItems = options.LineItems,
            SuccessUrl = options.SuccessUrl,
            CancelUrl = options.CancelUrl,
        });
    }

    /// <inheritdoc/>
    public virtual Task<PayCustomer> GetOrCreateCustomerAsync(PayCustomerOptions options)
    {
        string email = _payCustomerEmailProvider.ResolveCustomerEmail();
        return GetOrCreateCustomerAsync(email, options);
    }

    /// <inheritdoc/>
    public virtual async Task<PayCustomer> GetOrCreateCustomerAsync(string email, PayCustomerOptions options)
    {
        if (options.ProcessorName == PaymentProcessors.Fake && !options.AllowFake)
        {
            throw new PayDotNetException(string.Format("Processor '{0}' is not allowed", options.ProcessorName));
        }

        PayCustomer payCustomer = await _customerManager.GetOrCreateCustomerAsync(options.ProcessorName, email);
        if (!string.IsNullOrEmpty(options.PaymentMethodId))
        {
            await _paymentMethodManager.AddPaymentMethodAsync(payCustomer, new(options.PaymentMethodId, IsDefault: true));
        }

        return payCustomer;
    }

    /// <inheritdoc/>
    public virtual async Task<IPayment> SubscribeAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        if (_paymentMethodManager.IsPaymentMethodRequired(payCustomer) && payCustomer.DefaultPaymentMethod == null)
        {
            throw new PayDotNetException("Customer has no default payment method");
        }

        PaySubscriptionResult result = await _subscriptionManager.CreateSubscriptionAsync(payCustomer, options);

        // Let caller decide how to handle flow.
        return result.Payment;
    }
}