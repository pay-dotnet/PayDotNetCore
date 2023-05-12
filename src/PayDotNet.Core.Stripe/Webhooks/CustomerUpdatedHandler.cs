using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CustomerUpdatedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public CustomerUpdatedHandler(
        ICustomerManager customerManager,
        IPaymentMethodManager paymentMethodManager)
    {
        _customerManager = customerManager;
        _paymentMethodManager = paymentMethodManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Customer customer)
        {
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, customer.Id);
            if (payCustomer is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(customer.InvoiceSettings?.DefaultPaymentMethodId))
            {
                // Sync default card
                await _paymentMethodManager.SynchroniseAsync(payCustomer, customer.InvoiceSettings.DefaultPaymentMethodId);
            }
            else
            {
                // No default payment method set
                await _paymentMethodManager.ResetDefaultPaymentMethodsAsync(payCustomer);
            }
        }
    }
}