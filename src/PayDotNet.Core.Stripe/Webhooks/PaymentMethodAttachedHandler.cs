using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodAttachedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public PaymentMethodAttachedHandler(
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
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, paymentMethod.CustomerId);
            await _paymentMethodManager.SynchroniseAsync(payCustomer, paymentMethod.Id);
        }
    }
}