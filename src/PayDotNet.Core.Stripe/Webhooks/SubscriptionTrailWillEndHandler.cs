using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionTrailWillEndHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly IPayNotificationService _notificationService;
    private readonly ISubscriptionManager _subscriptionManager;

    public SubscriptionTrailWillEndHandler(
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
        if (@event.Data.Object is Subscription subscription)
        {
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, subscription.CustomerId);
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(payCustomer, subscription.Id);
            if (paySubscription is null)
            {
                return;
            }

            await _subscriptionManager.SynchroniseAsync(payCustomer, subscription.Id);
            if (paySubscription.IsOnTrial())
            {
                await _notificationService.OnSubscriptionTrialEndingAsync(payCustomer, paySubscription);
            }
            else if (paySubscription.IsTrialEnded())
            {
                await _notificationService.OnSubscriptionTrialEndedAsync(payCustomer, paySubscription);
            }
        }
    }
}