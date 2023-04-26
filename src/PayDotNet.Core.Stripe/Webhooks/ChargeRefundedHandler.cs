using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class ChargeRefundedHandler : IStripeWebhookHandler
{
    private readonly IChargeManager _chargeManager;

    public ChargeRefundedHandler(IChargeManager chargeManager)
    {
        _chargeManager = chargeManager;
    }

    public async Task HandleAsync(Event @event)
    {
        PayCharge payCharge = await _chargeManager.SynchroniseAsync(@event.Id);

        // TODO: decide what to do with emails;
    }
}
