using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodUpdatedHandler : IStripeWebhookHandler
{
    private readonly IPaymentMethodManager _paymentMethodManager;

    public PaymentMethodUpdatedHandler(IPaymentMethodManager paymentMethodManager)
    {
        _paymentMethodManager = paymentMethodManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is PaymentMethod paymentMethod)
        {
            if (paymentMethod.Customer is not null)
            {
                await _paymentMethodManager.SynchroniseAsync(paymentMethod.Id);
            }
            else
            {
                // If customer was removed, we should delete the payment method if it exists
                await _paymentMethodManager.DeleteByIdAsync(PaymentProcessors.Stripe, paymentMethod.Id);
            }
        }
    }
}