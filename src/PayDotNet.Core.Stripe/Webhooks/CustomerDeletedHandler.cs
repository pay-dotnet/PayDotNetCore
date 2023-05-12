using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CustomerDeletedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentMethodManager _paymentMethodManager;

    public CustomerDeletedHandler(
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentMethodManager paymentMethodManager)
    {
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
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

            // Mark all subscriptions as cancelled
            await _subscriptionManager.CancellAllAsync(payCustomer);

            // Remove all payment methods.
            await _paymentMethodManager.DeleteAllPaymentMethodsForCustomerAsync(payCustomer);

            // Mark customer as deleted.
            await _customerManager.SoftDeleteAsync(payCustomer);
        }
    }
}