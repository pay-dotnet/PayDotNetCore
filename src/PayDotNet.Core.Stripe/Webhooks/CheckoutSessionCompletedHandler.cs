using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;
using Stripe.Checkout;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CheckoutSessionCompletedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IChargeManager _chargeManager;
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

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Session session)
        {
            // If there is a client reference ID, make sure we have a PayCustomer record
            if (!string.IsNullOrEmpty(session.ClientReferenceId))
            {
                PayCustomer payCustomer = await _customerManager.GetOrCreateCustomerAsync(PaymentProcessors.Stripe, session.ClientReferenceId, session.CustomerEmail);
            }

            if (@event.Data.Object is PaymentIntent paymentIntent)
            {
                await _chargeManager.SynchroniseAsync(paymentIntent.Id);
            }

            if (@event.Data.Object is Subscription subscription)
            {
                await _chargeManager.SynchroniseAsync(subscription.Id);
            }
        }
    }
}