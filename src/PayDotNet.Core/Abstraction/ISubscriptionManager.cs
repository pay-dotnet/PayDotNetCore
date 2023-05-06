using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    Task<PaySubscription?> FindByIdAsync(string processorId, string customerId);

    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options);

    Task SynchroniseAsync(string processorId, PaySubscriptionResult? @object, PayCustomer payCustomer);

    Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options);

    Task CancelNowAsync(PayCustomer payCustomer, PaySubscription paySubscription);

    Task CancellAllAsync(PayCustomer payCustomer);
}