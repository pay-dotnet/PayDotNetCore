using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionCreatedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
