namespace PayDotNet.Core.Models;

public record PaySubscriptionResult(PaySubscription PaySubscription, IPayment Payment);

public record PayChargeResult(PayCharge PayCharge, IPayment Payment);