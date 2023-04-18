using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentMethodManager
{
    Task<PayPaymentMethod> AddPaymentMethodAsync(PaymentProcessorCustomer customer, string paymentMethodId, bool isDefault = false);
}