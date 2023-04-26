using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentIntentSucceededHandler : IStripeWebhookHandler
{
    private readonly IChargeManager _chargeManager;

    public PaymentIntentSucceededHandler(IChargeManager chargeManager)
    {
        _chargeManager = chargeManager;
    }

    public async Task HandleAsync(Event @event)
    {
        PaymentIntent? paymentIntent = @event.Data.Object as PaymentIntent;

        await _chargeManager.SynchroniseAsync(paymentIntent.Id, new());
    }
}