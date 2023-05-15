namespace PayDotNet.Core.Abstraction;

public interface IChargeManager
{
    /// <summary>
    /// Captures the charge based on the options.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="payCharge">The charge.</param>
    /// <param name="options">The options.</param>
    /// <returns>The payment to see if the capture was succeeded.</returns>
    Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeCaptureOptions options);

    /// <summary>
    /// Charges the customer with an amount and saves it into the store.
    /// The payment indicates if the charge was succesful or additional actions are required in case of SCA.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="options">The options.</param>
    /// <returns>The result.</returns>
    Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options);

    /// <summary>
    /// Gets the charge specified by the id from the store.
    /// </summary>
    /// <param name="processorId">The identifier.</param>
    /// <returns>The pay charge if found.</returns>
    Task<PayCharge?> GetAsync(string processorId);

    /// <summary>
    /// Refund the customer on the specified charge.
    /// Issues a CreditNote if there's an invoice, otherwise uses a Refund.
    /// This allows Tax to be handled properly
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="payCharge">The charge.</param>
    /// <param name="options">The options.</param>
    /// <returns>An awaitable task.</returns>
    Task RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options);

    /// <summary>
    /// Synchronises the charge to the store.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="processorId">The payment processor id for the charge.</param>
    /// <returns>The synchronised pay charge.</returns>
    Task<PayCharge?> SynchroniseAsync(PayCustomer payCustomer, string processorId);
}