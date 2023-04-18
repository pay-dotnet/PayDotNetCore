using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public record PaymentProcessorSubscription(string Id, string CustomerId, Dictionary<string, object?> Attributes, IPayment Payment)
{
    public virtual DateTime? GetTrialEndDate() => null;
};
