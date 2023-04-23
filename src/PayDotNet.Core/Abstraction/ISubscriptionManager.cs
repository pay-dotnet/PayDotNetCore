using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string priceId);

    Task SynchroniseAsync(string processorId, PaySubscriptionResult? @object, PayCustomer payCustomer);
}