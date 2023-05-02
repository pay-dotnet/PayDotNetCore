namespace PayDotNet.Core;

public record PayCheckoutChargeOptions(List<PayCheckoutLineItem> LineItems, string Mode = "payment", bool AllowPromotionCodes = false, string? SuccessUrl = null, string? CancelUrl = null)
    : PayCheckoutOptions(LineItems, Mode, AllowPromotionCodes, SuccessUrl, CancelUrl)
{
    public PayCheckoutChargeOptions(params PayCheckoutLineItem[] lineItems)
        : this(lineItems.ToList())
    {
    }

    public PayCheckoutChargeOptions(params PayCheckoutLineItemProductData[] products)
        : this(products.Select(product => new PayCheckoutLineItem(product)).ToList())
    {
    }

    public PayCheckoutChargeOptions(
        int unitAmount,
        string name,
        string currency,
        string? description = null,
        int quantity = 1)
        : this(new PayCheckoutLineItem(PriceData: new(name, unitAmount, currency, null, description), quantity))
    {
    }
}