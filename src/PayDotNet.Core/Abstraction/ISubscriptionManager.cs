using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    Task<PaySubscription?> FindByIdAsync(string processor, string processorId);

    Task CancellAllAsync(PayCustomer payCustomer);

    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string priceId);

    Task SynchroniseAsync(string processorId, PaySubscriptionResult? @object, PayCustomer payCustomer);
}