using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ICheckoutManager
{
    Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options);
}