using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentMethodManager
{
    Task<PayPaymentMethod> AddPaymentMethodAsync(PaymentProcessorCustomer customer, string paymentMethodId, bool isDefault = false);

    Task DeleteAllAsync(PayCustomer payCustomer);

    Task DeleteByIdAsync(string processorName, string processorId);

    Task<PayPaymentMethod> SynchroniseAsync(string paymentMethodId);

    Task UpdateAllAsync(bool isDefault);

    bool IsPaymentMethodRequired();
}