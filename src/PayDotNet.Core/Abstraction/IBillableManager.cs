using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IBillableManager
{
    Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName, bool allowFake = false);

    Task<PaySubscription> SubscribeAsync(PayCustomer payCustomer, string name = "default", string price = "default");
}