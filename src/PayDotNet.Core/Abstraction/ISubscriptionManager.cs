namespace PayDotNet.Core.Abstraction;

public interface ISubscriptionManager
{
    /// <summary>
    /// Cancels the subscription for this customer based on the options.
    /// </summary>
    /// <param name="payCustomer">The customer</param>
    /// <param name="paySubscription">The subscription that will be canceled.</param>
    /// <param name="options">The options</param>
    /// <returns>An awaitable task.</returns>
    Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options);

    /// <summary>
    /// Cancels all the subscriptions for this customer.
    /// </summary>
    /// <param name="payCustomer">The customer</param>
    /// <returns>An awaitable task.</returns>
    Task CancellAllAsync(PayCustomer payCustomer);

    /// <summary>
    /// Cancels the subscription for this customer immediatly.
    /// </summary>
    /// <param name="payCustomer">The customer</param>
    /// <param name="paySubscription">The subscription that will be canceled.</param>
    /// <returns>An awaitable task.</returns>
    Task CancelNowAsync(PayCustomer payCustomer, PaySubscription paySubscription);

    /// <summary>
    /// Creates a subscription for the customer, charges the customer and saves it into the store.
    /// The payment indicates if the charge was succesful or additional actions are required in case of SCA.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="options">The options.</param>
    /// <returns>The result.</returns>
    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options);

    /// <summary>
    /// Finds the subscription in the store.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="processorId">The id for the subscription of the payment processor.</param>
    /// <returns>The subscription if found.</returns>
    Task<PaySubscription?> FindByIdAsync(PayCustomer payCustomer, string processorId);

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
}