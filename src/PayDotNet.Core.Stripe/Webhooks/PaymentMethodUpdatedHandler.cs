using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodUpdatedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
