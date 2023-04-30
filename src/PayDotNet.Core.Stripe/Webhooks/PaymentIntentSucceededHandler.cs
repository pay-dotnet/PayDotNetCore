using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

/// <summary>
/// This webhook does NOT send notifications because stripe sends both
/// `charge.succeeded` and `payment_intent.succeeded` events.
///
/// We use `charge.succeeded` as the single place to send notifications
/// </summary>
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
        if (paymentIntent is null)
        {
            return;
        }

        await _chargeManager.SynchroniseAsync(PaymentProcessors.Stripe, paymentIntent.LatestChargeId, paymentIntent.CustomerId);
    }
}