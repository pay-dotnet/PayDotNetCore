using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionTrailWillEndHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
