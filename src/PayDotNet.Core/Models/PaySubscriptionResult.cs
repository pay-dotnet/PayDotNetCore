namespace PayDotNet.Core.Models;

public record PaySubscriptionResult(PaySubscription PaySubscription, IPayment? Payment);
