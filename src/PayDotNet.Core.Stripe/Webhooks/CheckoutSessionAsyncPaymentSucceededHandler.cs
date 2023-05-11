using PayDotNet.Core.Abstraction;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CheckoutSessionAsyncPaymentSucceededHandler : CheckoutSessionCompletedHandler
{
    public CheckoutSessionAsyncPaymentSucceededHandler(ICustomerManager customerManager, IChargeManager chargeManager, ISubscriptionManager subscriptionManager)
        : base(customerManager, chargeManager, subscriptionManager)
    {
    }

    public override Task HandleAsync(Event @event)
    {
        return base.HandleAsync(@event);
    }
}