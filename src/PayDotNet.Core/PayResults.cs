namespace PayDotNet.Core;

public record PaySubscriptionResult(PaySubscription PaySubscription, IPayment Payment);

public record PayChargeResult(PayCharge? PayCharge, IPayment Payment);

public record PayChargeRefundResult(PayChargeRefund Refund);

public record PayCheckoutResult(string Id, Uri CheckoutUrl, Uri SuccessUrl, string Mode);

public record PayCustomerResult(string ProcessorId, string Account);