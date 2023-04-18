using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Stripe;

public record StripePaymentProcessorSubscription(
    string Id,
    string CustomerId,
    Dictionary<string, object?> Attributes,
    IPayment Payment) : PaymentProcessorSubscription(Id, CustomerId, Attributes, Payment)
{
    public override DateTime? GetTrialEndDate()
    {
        throw new NotImplementedException();
    }
}