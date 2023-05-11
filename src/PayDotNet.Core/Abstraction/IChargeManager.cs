using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IChargeManager
{
    /// <summary>
    /// Synchronises the charge to the store.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="processorId">The payment processor id for the charge.</param>
    /// <returns>The synchronised pay charge.</returns>
    Task<PayCharge?> SynchroniseAsync(PayCustomer payCustomer, string processorId);

    Task<PayCharge?> GetAsync(string processorId);

    /// <summary>
    /// Charges the customer with an amount and saves it into the store.
    /// The payment indicates if the charge was succesful or additional actions are required in case of SCA.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="options">The options.</param>
    /// <returns>The result.</returns>
    Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options);

    Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeOptions options);

    Task RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options);

    Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge);
}