namespace PayDotNet.Core.Models;

public class PayChargeLineItem
{
    public DateTime PeriodEnd { get; set; }

    public DateTime PeriodStart { get; set; }

    public bool IsProration { get; set; }

    public string ProcessorId { get; set; }

    public string Description { get; set; }

    public string PriceId { get; set; }

    public int Quantity { get; set; }

    public int? UnitAmount { get; set; }

    public int Amount { get; set; }

    public virtual ICollection<string> Discounts { get; set; } = new List<string>();

    public virtual ICollection<PayChargeTaxAmount> TaxAmounts { get; set; }
}