using PayDotNet.Core.Models;

namespace PayDotNet.Core.Services;

public interface IPaymentProcessorService
{
    Task<PaymentProcessorPaymentMethod> AttachPaymentMethodAsync(string processorId, string paymentMethodId, bool isDefault);

    Task<PaymentProcessorCustomer?> GetCustomerAsync(string processorId);

    Task<PaymentProcessorCustomer> CreateCustomerAsync(string email, Dictionary<string, string> attributes);

    Task<PaymentProcessorCustomer> FindCustomerAsync(string processorId);

    Task<PaySubscriptionResult?> GetSubscriptionAsync(string processorId, PayCustomer payCustomer);

    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string[] plans, Dictionary<string, object?> attributes);

    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string plan, Dictionary<string, object?> attributes);

    Task<PayCharge> GetChargeAsync(string processorId);

    Task<IPayment> GetPaymentAsync(string processorId);

    bool IsPaymentMethodRequired { get; }
}