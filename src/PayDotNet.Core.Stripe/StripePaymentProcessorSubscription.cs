using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Stripe;

public class StripePaymentProcessorSubscription : PaymentProcessorSubscription
{
    public List<object> SubscriptionItems { get; set; } = new();

    public override void Map(PaySubscription paySubscription)
    {
    }
}