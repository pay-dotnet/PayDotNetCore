using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionDeletedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
