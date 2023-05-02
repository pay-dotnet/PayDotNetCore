namespace PayDotNet.Core;

public record PayAuthorizeChargeOptions(int Amount, string Currency, string? PaymentMethodId = null)
    : PayChargeOptions(Amount, Currency, PaymentMethodId, CaptureMethod: "manual");
