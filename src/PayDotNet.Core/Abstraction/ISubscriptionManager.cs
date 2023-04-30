using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    Task<PaySubscription?> FindByIdAsync(string processorId, string customerId);

    Task CancellAllAsync(PayCustomer payCustomer);

    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string priceId, string name);

    Task SynchroniseAsync(string processorId, PaySubscriptionResult? @object, PayCustomer payCustomer);
}