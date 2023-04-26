using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionUpdatedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}