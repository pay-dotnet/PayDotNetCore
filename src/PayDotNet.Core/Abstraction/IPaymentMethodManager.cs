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

    /// <summary>
    /// Deletes all the payment methods for this customer.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <returns>An awaitable task.</returns>
    Task DeleteAllPaymentMethodsForCustomerAsync(PayCustomer payCustomer);

    /// <summary>
    /// Deletes the payment method from the store.
    /// </summary>
    /// <param name="processorName">The payment processor name.</param>
    /// <param name="processorId">The id of the payment method in the payment processor.</param>
    /// <returns>An awaitable task.</returns>
    Task DeleteByIdAsync(string processorName, string processorId);

    /// <summary>
    /// Check if the payment method is required for the payment processor.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <returns>True or false.</returns>
    bool IsPaymentMethodRequired(PayCustomer payCustomer);

    /// <summary>
    /// Resets all the payment methods to no longer be default.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <returns>An awaitable task.</returns>
    Task ResetDefaultPaymentMethodsAsync(PayCustomer payCustomer);

    /// <summary>
    ///
    /// </summary>
    /// <param name="payCustomer"></param>
    /// <param name="paymentMethodId"></param>
    /// <returns>An awaitable task.</returns>
    Task SynchroniseAsync(PayCustomer payCustomer, string paymentMethodId);
}