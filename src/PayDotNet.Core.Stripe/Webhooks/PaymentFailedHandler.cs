using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentFailedHandler : IStripeWebhookHandler
{
    private readonly ISubscriptionManager _subscriptionManager;

    public PaymentFailedHandler(
        ISubscriptionManager subscriptionManager)
    {
        _subscriptionManager = subscriptionManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Invoice invoice)
        {
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(PaymentProcessors.Stripe, invoice.Subscription.Id);
            if (paySubscription is null)
            {
                return;
            }

            // TODO: decide on sending emails
        }
    }
}
