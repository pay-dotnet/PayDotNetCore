using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Services;

public interface IPaymentProcessorService
{
    Task<PaymentProcessorPaymentMethod> AttachPaymentMethodAsync(string processorId, string paymentMethodId, bool isDefault);

    Task<PaymentProcessorCustomer?> GetCustomerAsync(string processorId);

    Task<PaymentProcessorCustomer> CreateCustomerAsync(string email, Dictionary<string, string> attributes);

    Task<PaymentProcessorCustomer> FindCustomerAsync(string processorId);

    Task<PaymentProcessorSubscription?> GetSubscriptionAsync(string subscriptionId);

    Task<PaymentProcessorSubscription> CreateSubscriptionAsync(PayCustomer customer, string plan, Dictionary<string, object?> attributes);
}