using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodDetachedHandler : IStripeWebhookHandler
{
    private readonly IPaymentMethodManager _paymentMethodManager;

    public PaymentMethodDetachedHandler(
        IPaymentMethodManager paymentMethodManager)
    {
        _paymentMethodManager = paymentMethodManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is PaymentMethod paymentMethod)
        {
            await _paymentMethodManager.DeleteByIdAsync(PaymentProcessors.Stripe, paymentMethod.Id);
        }
    }
}