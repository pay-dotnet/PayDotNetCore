using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Stripe.Webhooks;

public class CheckoutSessionAsyncPaymentSucceededHandler : CheckoutSessionCompletedHandler
{
    public CheckoutSessionAsyncPaymentSucceededHandler(ICustomerManager customerManager, IChargeManager chargeManager, ISubscriptionManager subscriptionManager)
        : base(customerManager, chargeManager, subscriptionManager)
    {
    }
}