using PayDotNet.Core.Models;

namespace PayDotNet.Core.Services;

public interface IPaymentProcessorService
{
    #region Customer

    Task<string> CreateCustomerAsync(PayCustomer payCustomer);

    #endregion Customer

    #region Payment method API

    Task<PayPaymentMethod> AttachPaymentMethodAsync(PayCustomer payCustomer, string paymentMethodId, bool isDefault);

    #endregion Payment method API

    #region Subscriptions API

    /// <summary>
    /// Creates a subscription for a given customer and options
    /// </summary>
    /// <param name="payCustomer">The customer, should have payment methods filled.</param>
    /// <param name="options">The options that can be used to create the subscription.</param>
    /// <returns>The result.</returns>
    /// <remarks>
    /// If customer has no default payment method, we MUST allow the subscription to be incomplete.
    /// Then the caller, can decide if they want to redirect to the payment form.
    /// </remarks>
    Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options);

    Task<PaySubscriptionResult?> GetSubscriptionAsync(string processorId);

    Task CancelAsync(PaySubscription paySubscription, PayCancelSubscriptionOptions options);

    #endregion Subscriptions API

    #region Charges API

    Task<PayCharge> GetChargeAsync(string processorId);

    Task<IPayment> CaptureAsync(PayCharge payCharge, PayChargeOptions options);

    Task<IPayment> GetPaymentAsync(string processorId);

    Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options);

    #endregion Charges API

    #region Checkout API

    Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options);

    #endregion Checkout API

    #region Refunds API

    Task<PayChargeRefund> RefundAsync(PayCharge payCharge, PayChargeRefundOptions options);

    Task IssueCreditNotesAsync(PayCharge payCharge, PayChargeRefundOptions options);

    Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge);

    #endregion Refunds API

    bool IsPaymentMethodRequired { get; }
}