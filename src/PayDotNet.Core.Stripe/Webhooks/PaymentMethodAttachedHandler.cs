using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodAttachedHandler : IStripeWebhookHandler
{
    private readonly IPaymentMethodManager _paymentMethodManager;

    public PaymentMethodAttachedHandler(IPaymentMethodManager paymentMethodManager)
    {
        _paymentMethodManager = paymentMethodManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is PaymentMethod paymentMethod)
        {
            await _paymentMethodManager.SynchroniseAsync(paymentMethod.Id);
        }
    }
}