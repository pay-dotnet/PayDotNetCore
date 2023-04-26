using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Stripe;

public class StripePaymentProcessorSubscription : PaymentProcessorSubscription
{
    public List<object> SubscriptionItems { get; set; } = new();

    public override void Map(PaySubscription paySubscription)
    {
    }
}