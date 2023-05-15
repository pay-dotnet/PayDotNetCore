namespace PayDotNet.Stores;

public interface ISubscriptionStore : IModelStore<PaySubscription>
{
    IQueryable<PaySubscription> Subscriptions { get; }
}