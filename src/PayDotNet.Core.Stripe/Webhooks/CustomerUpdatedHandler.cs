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
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(customer.Id, PaymentProcessors.Stripe);
            if (payCustomer is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(customer.InvoiceSettings?.DefaultPaymentMethodId))
            {
                await _paymentMethodManager.SynchroniseAsync(customer.InvoiceSettings.DefaultPaymentMethodId);
            }
            else
            {
                await _paymentMethodManager.UpdateAllAsync(isDefault: false);
            }
        }
    }
}
