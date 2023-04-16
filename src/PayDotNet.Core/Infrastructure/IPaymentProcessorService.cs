using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Infrastructure;

public interface IPaymentProcessorService
{
    Task<PaymentProcessorCustomer?> GetCustomerAsync(string processorId);

    Task<PaymentProcessorCustomer> CreateCustomerAsync(string email, Dictionary<string, string> attributes);

    Task<PaymentProcessorSubscription?> GetSubscriptionAsync(string subscriptionId);

    Task<PaymentProcessorSubscription> CreateSubscriptionAsync(PayCustomer customer, string plan, Dictionary<string, object?> attributes);
}
