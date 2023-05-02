namespace PayDotNet.Core;

public record PayCheckoutLineItem(string? PriceId, int Quantity, PayCheckoutLineItemProductData? PriceData = null)
{
    public PayCheckoutLineItem(string PriceId, int Quantity = 1)
        : this(PriceId, Quantity, null)
    {
    }

    public PayCheckoutLineItem(PayCheckoutLineItemProductData PriceData, int Quantity = 1)
        : this(null, Quantity, PriceData)
    {
    }
}