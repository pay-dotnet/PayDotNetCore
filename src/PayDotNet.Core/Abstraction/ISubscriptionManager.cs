using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    Task<PaySubscription?> FindByIdAsync(string processorId, string customerId);

    /// <summary>
    /// Creates a subscription for the customer, charges the customer and saves it into the store.
    /// The payment indicates if the charge was succesful or additional actions are required in case of SCA.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="options">The options.</param>
    /// <returns>The result.</returns>
    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options);

    /// <summary>
    /// Synchronises the subscription to the store. Use the overload if you already have the paySubscription.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="processorId"></param>
    /// <returns>An awaitable task.</returns>
    Task SynchroniseAsync(PayCustomer payCustomer, string processorId);

    /// <summary>
    /// Synchronises the subscription to the store.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="paySubscriptionResult">The pay subscription result.</param>
    /// <returns>An awaitable task.</returns>
    Task SynchroniseAsync(PayCustomer payCustomer, PaySubscriptionResult paySubscriptionResult);

    Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options);

    Task CancelNowAsync(PayCustomer payCustomer, PaySubscription paySubscription);

    Task CancellAllAsync(PayCustomer payCustomer);
}