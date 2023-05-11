using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentProcessorService
{
    string Name { get; }

    #region Customer

    Task<string> CreateCustomerAsync(PayCustomer payCustomer);

    #endregion Customer

    #region Payment method API

    Task<PayPaymentMethod?> GetPaymentMethodAsync(PayCustomer payCustomer, string processorId);

    /// <summary>
    /// Attaches the payment method to the customer in the PaymentProcessor.
    /// </summary>
    /// <param name="payCustomer">The pay customer for which this operation should take place.</param>
    /// <param name="options">The options.</param>
    /// <returns>The payment method.</returns>
    Task<PayPaymentMethod> AttachPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options);

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

    Task<PaySubscriptionResult?> GetSubscriptionAsync(PayCustomer payCustomer, string processorId);

    Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options);

    #endregion Subscriptions API

    #region Charges API

    /// <summary>
    /// Retrieves the charge based on the customer and processor id.
    /// If the customer is null, it should return a null.
    /// </summary>
    /// <param name="payCustomer">The customer.</param>
    /// <param name="processorId">The payment processor id for the charge.</param>
    /// <returns>The pay charge if found.</returns>
    Task<PayCharge?> GetChargeAsync(PayCustomer payCustomer, string processorId);

    Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeOptions options);

    Task<IPayment> GetPaymentAsync(PayCustomer payCustomer, string processorId);

    /// <summary>
    /// Charges the customer an amount. Returns a result that can be used to validate if payment was successful or an action is required.
    /// </summary>
    /// <param name="payCustomer">The pay customer for which this operation should take place.</param>
    /// <param name="options">The options.</param>
    /// <returns>The charge result.</returns>
    Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options);

    #endregion Charges API

    #region Checkout API

    Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options);

    #endregion Checkout API

    #region Refunds API

    Task<PayChargeRefund> RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options);

    Task IssueCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options);

    Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge);

    #endregion Refunds API

    bool IsPaymentMethodRequired(PayCustomer payCustomer);
}