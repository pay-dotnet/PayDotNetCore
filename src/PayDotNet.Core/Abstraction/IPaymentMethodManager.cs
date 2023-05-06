using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentMethodManager
{
    Task<PayPaymentMethod> AddPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options);

    Task DeleteAllAsync(PayCustomer payCustomer);

    Task DeleteByIdAsync(string processorName, string processorId);

    Task<PayPaymentMethod> SynchroniseAsync(string paymentMethodId);

    Task UpdateAllAsync(bool isDefault);

    bool IsPaymentMethodRequired(PayCustomer payCustomer);
}