using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class ChargeRefundedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IChargeManager _chargeManager;

    public ChargeRefundedHandler(
        ICustomerManager customerManager,
        IChargeManager chargeManager)
    {
        _customerManager = customerManager;
        _chargeManager = chargeManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Charge charge)
        {
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, charge.CustomerId);
            await _chargeManager.SynchroniseAsync(payCustomer, charge.Id);
        }

        // TODO: decide what to do with emails;
    }
}