using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ICheckoutManager
{
    /// <summary>
    /// Starts a checkout session for the user based on the options.
    /// </summary>
    /// <param name="payCustomer">The customer</param>
    /// <param name="options">The options</param>
    /// <returns>The result of this operation.</returns>
    Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options);
}