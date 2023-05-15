using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class PaymentFailedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPayNotificationService _notificationService;
    private readonly ISubscriptionManager _subscriptionManager;

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
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(payCustomer, invoice.SubscriptionId);
            if (paySubscription is null)
            {
                return;
            }

            await _notificationService.OnPaymentFailedAsync(payCustomer, paySubscription, null);
        }
    }
}