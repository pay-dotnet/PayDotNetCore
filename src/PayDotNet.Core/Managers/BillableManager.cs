﻿using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

/// <summary>
/// Implementation of the facade containing the primary operations for Pay
/// </summary>
public class BillableManager : IBillableManager
{
    private readonly IPayCustomerEmailResolverService _payCustomerEmailResolver;
    private readonly ICheckoutManager _checkoutManager;
    private readonly IChargeManager _chargeManager;
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public BillableManager(
        IPayCustomerEmailResolverService payCustomerEmailResolver,
        ICheckoutManager checkoutManager,
        IChargeManager chargeManager,
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentMethodManager paymentMethodManager)
    {
        _payCustomerEmailResolver = payCustomerEmailResolver;
        _checkoutManager = checkoutManager;
        _chargeManager = chargeManager;
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _paymentMethodManager = paymentMethodManager;
    }

    /// <inheritdoc/>
    public virtual Task<PayCustomer> GetOrCreateCustomerAsync(PayCustomerOptions options)
    {
        string email = _payCustomerEmailResolver.ResolveCustomerEmail();
        return GetOrCreateCustomerAsync(email, options);
    }

    /// <inheritdoc/>
    public virtual async Task<PayCustomer> GetOrCreateCustomerAsync(string email, PayCustomerOptions options)
    {
        PayCustomer payCustomer = await _customerManager.GetOrCreateCustomerAsync(email, options.ProcessorName);
        if (!string.IsNullOrEmpty(options.PaymentMethodId))
        {
            await _paymentMethodManager.AddPaymentMethodAsync(payCustomer, new(options.PaymentMethodId, IsDefault: true));
        }

        return payCustomer;
    }

    /// <inheritdoc/>
    public virtual async Task<IPayment> SubscribeAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        if (_paymentMethodManager.IsPaymentMethodRequired(payCustomer))
        {
            if (payCustomer.DefaultPaymentMethod == null)
            {
                throw new PayDotNetException("Customer has no default payment method");
            }
        }

        PaySubscriptionResult result = await _subscriptionManager.CreateSubscriptionAsync(payCustomer, options);

        // Let caller decide how to handle flow.
        return result.Payment;
    }

    /// <inheritdoc/>
    public virtual async Task<IPayment> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        if (!string.IsNullOrEmpty(options.PaymentMethodId))
        {
            await _paymentMethodManager.AddPaymentMethodAsync(payCustomer, new(options.PaymentMethodId, IsDefault: true));
        }

        return await _chargeManager.ChargeAsync(payCustomer, options);
    }

    /// <inheritdoc/>
    public virtual Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        return _checkoutManager.CheckoutAsync(payCustomer, options);
    }

    /// <inheritdoc/>
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
    public virtual Task AuthorizeAsync(PayCustomer payCustomer, PayAuthorizeChargeOptions options)
    {
        return ChargeAsync(payCustomer, options);
    }
}