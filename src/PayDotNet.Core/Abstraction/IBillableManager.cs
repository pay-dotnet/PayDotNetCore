namespace PayDotNet.Core.Abstraction;

public interface IBillableManager
{
    Task<PayCustomer> ResolveCustomerAsync(PayCustomer customer);

    Task<PayCustomer> SetPaymentProcessorAsync(string customerId, string processorName);

    Task<PaySubscription> SubscribeAsync(PayCustomer customer, string name = "default", string plan = "default");
}
