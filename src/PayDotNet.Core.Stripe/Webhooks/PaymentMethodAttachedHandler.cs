using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodAttachedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
