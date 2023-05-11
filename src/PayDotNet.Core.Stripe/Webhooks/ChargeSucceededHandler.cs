using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class ChargeSucceededHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IChargeManager _chargeManager;
    private readonly IPayNotificationService _notificationService;

    public ChargeSucceededHandler(
        ICustomerManager customerManager,
        IChargeManager chargeManager,
        IPayNotificationService notificationService)
    {
        _customerManager = customerManager;
        _chargeManager = chargeManager;
        _notificationService = notificationService;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Charge charge)
        {
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, charge.CustomerId);
            PayCharge? payCharge = await _chargeManager.SynchroniseAsync(payCustomer, charge.Id);
            await _notificationService.OnChargeSucceededAsync(payCustomer, payCharge);
        }
    }
}