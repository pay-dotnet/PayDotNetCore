using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class SubscriptionCreatedHandler : IStripeWebhookHandler
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public SubscriptionCreatedHandler(
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
            PayCustomer payCustomer = await _customerManager.FindByIdAsync(PaymentProcessors.Stripe, subscription.CustomerId);
            await _subscriptionManager.SynchroniseAsync(payCustomer, subscription.Id);
        }
    }
}