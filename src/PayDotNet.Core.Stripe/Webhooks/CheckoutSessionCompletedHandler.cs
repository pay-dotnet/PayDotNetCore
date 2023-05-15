﻿using PayDotNet.Core.Abstraction;
using Stripe;
using Stripe.Checkout;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CheckoutSessionCompletedHandler : IStripeWebhookHandler
{
    private readonly IChargeManager _chargeManager;
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public CheckoutSessionCompletedHandler(
        ICustomerManager customerManager,
        IChargeManager chargeManager,
        ISubscriptionManager subscriptionManager)
    {
        _customerManager = customerManager;
        _chargeManager = chargeManager;
        _subscriptionManager = subscriptionManager;
    }

    public virtual async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Session session)
        {
            PayCustomer payCustomer = null;
            // If there is a client reference ID, make sure we have a PayCustomer record
            if (!string.IsNullOrEmpty(session.ClientReferenceId))
            {
                payCustomer = await _customerManager.GetOrCreateCustomerAsync(PaymentProcessors.Stripe, session.ClientReferenceId, session.CustomerEmail);
            }

            if (@event.Data.Object is PaymentIntent paymentIntent)
            {
                PayCharge? _ = await _chargeManager.SynchroniseAsync(payCustomer, paymentIntent.LatestChargeId);
            }

            if (@event.Data.Object is Subscription subscription)
            {
                await _subscriptionManager.SynchroniseAsync(payCustomer, subscription.Id);
            }
        }
    }
}