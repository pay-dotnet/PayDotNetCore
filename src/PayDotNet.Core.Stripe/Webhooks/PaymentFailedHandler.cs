using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentFailedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPayNotificationService _notificationService;

    public PaymentFailedHandler(
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPayNotificationService notificationService)
    {
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _notificationService = notificationService;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Invoice invoice)
        {
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, invoice.CustomerId);
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(invoice.SubscriptionId, payCustomer.Id);
            if (paySubscription is null)
            {
                return;
            }

            await _notificationService.OnPaymentFailedAsync(payCustomer, paySubscription, null);
        }
    }
}