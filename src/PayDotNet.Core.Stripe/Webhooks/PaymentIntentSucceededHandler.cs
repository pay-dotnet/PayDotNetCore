using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

/// <summary>
/// This webhook does NOT send notifications because stripe sends both
/// `charge.Succeeded` and `payment_intent.Succeeded` events.
///
/// We use `charge.Succeeded` as the single place to send notifications
/// </summary>
public class PaymentIntentSucceededHandler : IStripeWebhookHandler
{
    private readonly IChargeManager _chargeManager;
    private readonly ICustomerManager _customerManager;

    public PaymentIntentSucceededHandler(
        ICustomerManager customerManager,
        IChargeManager chargeManager)
    {
        _customerManager = customerManager;
        _chargeManager = chargeManager;
    }

    public async Task HandleAsync(Event @event)
    {
        PaymentIntent? paymentIntent = @event.Data.Object as PaymentIntent;
        if (paymentIntent is null)
        {
            return;
        }

        PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, paymentIntent.CustomerId);
        PayCharge? _ = await _chargeManager.SynchroniseAsync(payCustomer, paymentIntent.LatestChargeId);
    }
}