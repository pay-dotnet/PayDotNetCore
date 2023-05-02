namespace PayDotNet.Core;

public record PayCheckoutOptions(List<PayCheckoutLineItem> LineItems, string Mode = "payment", bool AllowPromotionCodes = false, string? SuccessUrl = null, string? CancelUrl = null)
{
    public PayCheckoutOptions(string mode)
        : this(new(), mode)
    {
    }

    public PayCheckoutOptions(string lineItem, int quantity = 1, bool allowPromotionCodes = false)
        : this(LineItems: new()
        {
            new PayCheckoutLineItem(PriceId: lineItem, Quantity: quantity)
        })
    {
    }

    public PayCheckoutOptions(params string[] lineItems)
        : this(LineItems: lineItems.Select(l => new PayCheckoutLineItem(PriceId: l)).ToList())
    {
    }
}