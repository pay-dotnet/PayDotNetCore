using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CheckoutSessionAsyncPaymentSucceededHandler : CheckoutSessionCompletedHandler
{
    public CheckoutSessionAsyncPaymentSucceededHandler(IChargeManager chargeManager, ISubscriptionManager subscriptionManager)
        : base(chargeManager, subscriptionManager)
    {
    }
}
