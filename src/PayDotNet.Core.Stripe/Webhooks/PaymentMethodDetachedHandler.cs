using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodDetachedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public PaymentMethodDetachedHandler(
        ICustomerManager customerManager,
        IPaymentMethodManager paymentMethodManager)
    {
        _customerManager = customerManager;
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