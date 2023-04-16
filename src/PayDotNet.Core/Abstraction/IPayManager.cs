namespace PayDotNet.Core.Abstraction;

public interface IPayManager
{
    Task<PayCustomer> ResolveCustomerAsync(PayCustomer customer);

    Task<PaySubscription> SubscribeAsync(PayCustomer customer, string name = "default", string plan = "default");
}
