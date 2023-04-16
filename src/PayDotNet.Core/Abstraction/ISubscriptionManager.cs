namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    public Task<PaySubscription> CreateAsync(string name, string processorId, string processorPlan, PayStatus status, DateTime? trailEndsAt, Dictionary<string, string> Metadata);

    public Task<PaySubscription?> SynchroniseAsync(string subscriptionId, object @object = null, string name = "", string stripeAcount = "", int attempt = 0, int retries = 1);
}
