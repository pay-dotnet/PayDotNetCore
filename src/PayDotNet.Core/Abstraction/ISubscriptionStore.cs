using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionStore : IModelStore<PaySubscription>
{
    IQueryable<PaySubscription> Subscriptions { get; }
}