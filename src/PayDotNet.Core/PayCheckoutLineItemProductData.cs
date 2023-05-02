namespace PayDotNet.Core;

public record PayCheckoutLineItemProductData(string Name, int UnitAmount, string Currency, List<string>? Images, string? Description);