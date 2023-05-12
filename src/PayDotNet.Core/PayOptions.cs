namespace PayDotNet.Core;

public record PaySubscribeOptionsItem(string PriceId, int Quantity = 1);

/// <summary>
/// The options for creating a one-time charge to the customer.
/// </summary>
/// <param name="Amount">The amount.</param>
/// <param name="Currency">The currency.</param>
/// <param name="PaymentMethodId">If specified, make this PaymentMethod the new default.</param>
/// <param name="CaptureMethod">If specified, change the capture method for example to place a hold on payment method.</param>
public record PayChargeOptions(
    int Amount,
    string Currency,
    string? PaymentMethodId = null,
    string CaptureMethod = "automatic")
{
}

/// <summary>
/// The options for capturing a previous authorized charge.
/// </summary>
/// <param name="AmountToCapture">(Optional) the amount to capture of the authorized amount. Leaving empty means everything will be captured.</param>
public record PayChargeCaptureOptions(int? AmountToCapture);

public record PayAuthorizeChargeOptions(int Amount, string Currency, string? PaymentMethodId = null)
    : PayChargeOptions(Amount, Currency, PaymentMethodId, CaptureMethod: "manual");

/// <summary>
///
/// </summary>
/// <param name="Amount">The amount</param>
/// <param name="Description">Only used for credit notes in invoice.</param>
/// <param name="RefundApplicationFee"></param>
public record PayChargeRefundOptions(int Amount, string? Description = null, bool RefundApplicationFee = false);

/// <summary>
/// </summary>
/// <param name="PaymentMethodId"></param>
/// <param name="IsDefault"></param>
public record PayPaymentMethodOptions(string PaymentMethodId, bool IsDefault);