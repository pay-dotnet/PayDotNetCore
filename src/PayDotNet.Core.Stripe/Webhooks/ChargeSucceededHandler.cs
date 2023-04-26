using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
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
        PayCharge payCharge = await _chargeManager.SynchroniseAsync(@event.Id);
        // TODO: decide what to do with emails;
    }
}
