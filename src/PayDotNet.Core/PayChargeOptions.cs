namespace PayDotNet.Core;

/// <summary>
/// The options for creating a one-time charge to the customer.
/// </summary>
/// <param name="Amount">The amount.</param>
/// <param name="Currency">The currency.</param>
/// <param name="PaymentMethodId">If specified, make this PaymentMethod the new default.</param>
/// <param name="CaptureMethod">If specified, change the capture method for example to place a hold on payment method.</param>
public record PayChargeOptions(int Amount, string Currency, string? PaymentMethodId = null, string CaptureMethod = "automatic")
{
}