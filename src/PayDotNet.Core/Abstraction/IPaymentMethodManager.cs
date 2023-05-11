using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentMethodManager
{
    /// <summary>
    /// Add or attaches the payment method and saves it into the store
    /// </summary>
    /// <param name="payCustomer">The customer to which the payment method is related.</param>
    /// <param name="options">The options.</param>
    /// <returns>The resulting PaymentMethod.</returns>
    Task<PayPaymentMethod> AddPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options);

    Task DeleteAllAsync(PayCustomer payCustomer);

    Task DeleteByIdAsync(string processorName, string processorId);

    Task SynchroniseAsync(PayCustomer payCustomer, string paymentMethodId);

    Task UpdateAllAsync(PayCustomer payCustomer, bool isDefault);

    bool IsPaymentMethodRequired(PayCustomer payCustomer);
}