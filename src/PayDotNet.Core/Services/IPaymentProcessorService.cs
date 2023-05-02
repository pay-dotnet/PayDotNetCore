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

    Task<Uri> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options);

    #endregion Checkout API

    #region Refunds API

    Task RefundAsync(PayCharge payCharge, PayChargeRefundOptions options);

    Task IssueCreditNotesAsync(PayCharge payCharge, PayChargeRefundOptions options);

    Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge);

    #endregion Refunds API

    bool IsPaymentMethodRequired { get; }
}