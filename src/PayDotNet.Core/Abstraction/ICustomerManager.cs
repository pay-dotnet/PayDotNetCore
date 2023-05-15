using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ICustomerManager
{
    /// <summary>
    /// Finds the customer in the store by email.
    /// </summary>
    /// <param name="processorName">The payment processor name.</param>
    /// <param name="email">The email.</param>
    /// <returns>The pay customer.</returns>
    Task<PayCustomer?> FindByEmailAsync(string processorName, string email);

    /// <summary>
    /// Finds the customer in the store by id.
    /// </summary>
    /// <param name="processorName">The payment processor name.</param>
    /// <param name="processorId">The customer identifier from the payment processor.</param>
    /// <returns>The pay customer.</returns>
    Task<PayCustomer?> FindByIdAsync(string processorName, string processorId);

    /// <summary>
    /// Get or creates the customer both in the store as well as in the payment processor.
    /// If it already exists, there are no API calls to the payment processor.
    ///
    /// Note: This method is for when the payment processor pushes the customer first through e.g. webhooks.
    /// </summary>
    /// <param name="processorName">The payment processor name.</param>
    /// <param name="processorId">The customer identifier from the payment processor.</param>
    /// <param name="email">The email.</param>
    /// <returns>The pay customer.</returns>
    Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string processorId, string email);

    /// <summary>
    /// Get or creates the customer both in the store as well as in the payment processor.
    /// If it already exists, there are no API calls to the payment processor.
    /// </summary>
    /// <param name="processorName">The payment processor name.</param>
    /// <param name="email">The email.</param>
    /// <returns>The pay customer.</returns>
    Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string email);

    /// <summary>
    /// Soft deletes the customer in the store, since we don't want to delete the customer and its history.
    /// </summary>
    /// <param name="payCustomer">The pay customer.</param>
    /// <returns>An awaitable task.</returns>
    Task SoftDeleteAsync(PayCustomer payCustomer);
}