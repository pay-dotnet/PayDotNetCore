namespace PayDotNet.Core;

public record PayChargeRefundOptions(int Amount, string? Description = null, bool RefundApplicationFee = false);