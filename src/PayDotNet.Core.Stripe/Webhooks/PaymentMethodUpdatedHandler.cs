using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentMethodUpdatedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public PaymentMethodUpdatedHandler(
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
            if (paymentMethod.Customer is not null)
            {
                PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, paymentMethod.CustomerId);
                if (payCustomer is null)
                {
                    // If there is a client reference ID, make sure we have a PayCustomer record
                    payCustomer = await _customerManager.GetOrCreateCustomerAsync(PaymentProcessors.Stripe, paymentMethod.CustomerId, paymentMethod.Customer.Email);
                }
                await _paymentMethodManager.SynchroniseAsync(payCustomer, paymentMethod.Id);
            }
            else
            {
                // If customer was removed, we should delete the payment method if it exists
                await _paymentMethodManager.DeleteByIdAsync(PaymentProcessors.Stripe, paymentMethod.Id);
            }
        }
    }
}