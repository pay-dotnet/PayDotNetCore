using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionTrailWillEndHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public SubscriptionTrailWillEndHandler(
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager)
    {
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Subscription subscription)
        {
            PayCustomer? payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, subscription.CustomerId);
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(subscription.Id, payCustomer.Id);
            if (paySubscription is null)
            {
                return;
            }

            await _subscriptionManager.SynchroniseAsync(subscription.Id, null, payCustomer);

            if (paySubscription.IsOnTrial())
            {
                // TODO: Send email for trial will end
            }
            else if (paySubscription.IsTrialEnded())
            {
                // TODO: Send email for trial has ended.
            }
        }
    }
}