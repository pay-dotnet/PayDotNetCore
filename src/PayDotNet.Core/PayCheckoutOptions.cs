namespace PayDotNet.Core;

/// <summary>
///
/// </summary>
/// <param name="LineItems"></param>
/// <param name="Mode"></param>
/// <param name="AllowPromotionCodes">
///
/// Note: only works when the mode is <code>payment</code> or <code>subscription</code>.
/// </param>
/// <param name="SuccessUrl"></param>
/// <param name="CancelUrl"></param>
public record PayCheckoutOptions(List<PayCheckoutLineItem> LineItems, string Mode = "payment", string? ClientReferenceId = null, bool AllowPromotionCodes = false, string? SuccessUrl = null, string? CancelUrl = null)
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

public record PayCheckoutChargeOptions(List<PayCheckoutLineItem> LineItems, string Mode = "payment", string? ClientReferenceId = null, bool AllowPromotionCodes = false, string? SuccessUrl = null, string? CancelUrl = null)
    : PayCheckoutOptions(LineItems, Mode, ClientReferenceId, AllowPromotionCodes, SuccessUrl, CancelUrl)
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

public record PayCheckoutLineItemProductData(string Name, int UnitAmount, string Currency, List<string>? Images = null, string? Description = null);