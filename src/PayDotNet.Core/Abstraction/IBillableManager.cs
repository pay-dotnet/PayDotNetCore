using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IBillableManager
{
    Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName, bool allowFake = false);

    Task<PayCustomer> InitializeCustomerAsync(PayCustomer payCustomer, string? paymentMethodId);

    Task<PaySubscription> SubscribeAsync(PayCustomer customer, string name = "default", string price = "default");
}