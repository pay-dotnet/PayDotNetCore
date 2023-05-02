using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

/// <summary>
/// Implementation of the facade containing the primary operations for Pay
/// </summary>
public class BillableManager : IBillableManager
{
    private readonly ICheckoutManager _checkoutManager;
    private readonly IChargeManager _chargeManager;
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public BillableManager(
        ICheckoutManager checkoutManager,
        IChargeManager chargeManager,
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentMethodManager paymentMethodManager)
    {
        _checkoutManager = checkoutManager;
        _chargeManager = chargeManager;
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _paymentMethodManager = paymentMethodManager;
    }

    /// <inheritdoc/>
    public async Task<PayCustomer> GetOrCreateCustomerAsync(string email, PayCustomerOptions options)
    {
        if (options.ProcessorName.Equals(PaymentProcessors.Fake, StringComparison.OrdinalIgnoreCase) && !options.AllowFake)
        {
            throw new PayDotNetException($"Processor `{options.ProcessorName}` is not allowed");
        }

        PayCustomer payCustomer = await _customerManager.GetOrCreateCustomerAsync(email, options.ProcessorName);
        if (!string.IsNullOrEmpty(options.PaymentMethodId))
        {
            await _paymentMethodManager.AddPaymentMethodAsync(payCustomer, options.PaymentMethodId, isDefault: true);
        }

        return payCustomer;
    }

    /// <inheritdoc/>
    public async Task<IPayment> SubscribeAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        if (_paymentMethodManager.IsPaymentMethodRequired())
        {
            PayPaymentMethod? paymentMethod = payCustomer.PaymentMethods.FirstOrDefault(p => p.IsDefault);
            if (paymentMethod == null)
            {
                throw new PayDotNetException("Customer has no default payment method");
            }
        }

        PaySubscriptionResult result = await _subscriptionManager.CreateSubscriptionAsync(payCustomer, options);

        // Let caller decide how to handle flow.
        return result.Payment;
    }

    /// <inheritdoc/>
    public async Task<IPayment> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        if (!string.IsNullOrEmpty(options.PaymentMethodId))
        {
            await _paymentMethodManager.AddPaymentMethodAsync(payCustomer, options.PaymentMethodId, isDefault: true);
        }

        return await _chargeManager.ChargeAsync(payCustomer, options);
    }

    /// <inheritdoc/>
    public Task<Uri> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        return _checkoutManager.CheckoutAsync(payCustomer, options);
    }

    /// <inheritdoc/>
    public Task<Uri> CheckoutChargeAsync(PayCustomer payCustomer, PayCheckoutChargeOptions options)
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
    public Task AuthorizeAsync(PayCustomer payCustomer, PayAuthorizeChargeOptions options)
    {
        return ChargeAsync(payCustomer, options);
    }
}