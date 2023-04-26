using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

// TODO: merchants.
public class AccountUpdatedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
