namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: unique (customerid, processorid)
/// </summary>
public class PayCharge : Timestamps
{
    public string CustomerId { get; set; }

    public string? SubscriptionId { get; set; }

    public string ProcessorId { get; set; }

    public int Amount { get; set; }

    public string? Currency { get; set; }

    public int? ApplicationFeeAmount { get; set; }

    public int? AmountRefunded { get; set; }

    #region Additional Properties

    public string? Bank { get; set; }

    public string? Brand { get; set; }

    public int AmountCaptured { get; set; }

    public string? ExpirationMonth { get; set; }

    public string? ExpirationYear { get; set; }

    public string PaymentIntentId { get; set; }

    public string PaymentMethodType { get; set; }

    public string ReceiptUrl { get; set; }

    public string? Last4 { get; set; }

    public PayStatus Status { get; set; }

    public virtual ICollection<PayChargeLineItem> LineItems { get; set; } = new List<PayChargeLineItem>();

    public virtual ICollection<string> Discounts { get; set; } = new List<string>();

    public virtual ICollection<PayChargeTaxAmount> TotalTaxAmounts { get; set; } = new List<PayChargeTaxAmount>();

    public virtual ICollection<PayChargeTotalDiscount> TotalDiscountAmounts { get; set; } = new List<PayChargeTotalDiscount>();

    public virtual ICollection<PayChargeRefund> Refunds { get; set; } = new List<PayChargeRefund>();
    public DateTime PeriodEnd { get; set; }
    public DateTime PeriodStart { get; set; }
    public string? InvoiceId { get; set; }
    public int Subtotal { get; set; }
    public int? Tax { get; set; }

    #endregion Additional Properties
}

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

public class PayChargeRefund
{
    public string ProcessorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }
}

public class PayChargeTaxAmount
{
    public int Amount { get; set; }

    public string Description { get; set; }
}

public class PayChargeTotalDiscount
{
    public string DiscountId { get; set; }

    public int Amount { get; set; }

    public string Description { get; set; }
}