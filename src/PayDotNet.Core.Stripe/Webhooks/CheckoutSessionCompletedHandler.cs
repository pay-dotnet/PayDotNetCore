using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CheckoutSessionCompletedHandler : IStripeWebhookHandler
{
    private readonly IChargeManager _chargeManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public CheckoutSessionCompletedHandler(
        IChargeManager chargeManager,
        ISubscriptionManager subscriptionManager)
    {
        _chargeManager = chargeManager;
        _subscriptionManager = subscriptionManager;
    }

    public async Task HandleAsync(Event @event)
    {
        // TODO: get pay customer???

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
