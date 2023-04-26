using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodDetachedHandler : IStripeWebhookHandler
{
    public Task HandleAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
