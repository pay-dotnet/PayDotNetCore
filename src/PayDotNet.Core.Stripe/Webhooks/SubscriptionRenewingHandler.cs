using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

/// <summary>
/// Handles `invoice.upcoming` webhook from Stripe.
/// Occurs X number of days before a invoice is scheduled to create an invoice that is automatically charged—where X is determined by your subscriptions settings.
///
/// Note: The received Invoice object will not have an invoice ID.
/// </summary>
public class SubscriptionRenewingHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPayNotificationService _notificationService;
    private readonly ISubscriptionManager _subscriptionManager;

    public SubscriptionRenewingHandler(
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
        if (@event.Data.Object is Invoice invoice && !string.IsNullOrEmpty(invoice.SubscriptionId))
        {
            PayCustomer payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, invoice.CustomerId);
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(payCustomer, invoice.SubscriptionId);
            if (paySubscription is null)
            {
                return;
            }

            await _notificationService.OnSubscriptionRenewingAsync(payCustomer, paySubscription);
        }
    }
}