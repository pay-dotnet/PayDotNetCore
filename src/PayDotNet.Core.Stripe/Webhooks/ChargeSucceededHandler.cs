using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class ChargeSucceededHandler : IStripeWebhookHandler
{
    private readonly IChargeManager _chargeManager;

    public ChargeSucceededHandler(IChargeManager chargeManager)
    {
        _chargeManager = chargeManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Charge charge)
        {
            await _chargeManager.SynchroniseAsync(PaymentProcessors.Stripe, charge.Id, charge.CustomerId);
            // TODO: decide what to do with emails;
        }
    }
}