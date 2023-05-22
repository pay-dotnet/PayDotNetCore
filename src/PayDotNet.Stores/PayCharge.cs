namespace PayDotNet.Stores;

public class PayCharge : Timestamps
{
    public int Amount { get; set; }

    public int? AmountRefunded { get; set; }

    public int? ApplicationFeeAmount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string ProcessorId { get; set; } = string.Empty;

    public string SubscriptionId { get; set; } = string.Empty;

    #region Additional Properties

    public int AmountCaptured { get; set; }

    public string? Bank { get; set; }

    public string? Brand { get; set; }

    public virtual List<string> Discounts { get; set; } = new List<string>();

    public string? ExpirationMonth { get; set; }

    public string? ExpirationYear { get; set; }

    public string? InvoiceId { get; set; }

    public string? Last4 { get; set; }

    public virtual ICollection<PayChargeLineItem> LineItems { get; set; } = new List<PayChargeLineItem>();

    public string PaymentIntentId { get; set; } = string.Empty;

    public string PaymentMethodType { get; set; } = string.Empty;

    public DateTime PeriodEnd { get; set; }

    public DateTime PeriodStart { get; set; }

    public string ReceiptUrl { get; set; } = string.Empty;

    public virtual ICollection<PayChargeRefund> Refunds { get; set; } = new List<PayChargeRefund>();

    public PayStatus Status { get; set; }

    public int Subtotal { get; set; }

    public int? Tax { get; set; }

    public virtual ICollection<PayChargeTotalDiscount> TotalDiscountAmounts { get; set; } = new List<PayChargeTotalDiscount>();

    public virtual ICollection<PayChargeTaxAmount> TotalTaxAmounts { get; set; } = new List<PayChargeTaxAmount>();

    #endregion Additional Properties
}

public class PayChargeLineItem
{
    public int Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public virtual List<string> Discounts { get; set; } = new List<string>();

    public bool IsProration { get; set; }

    public DateTime PeriodEnd { get; set; }

    public DateTime PeriodStart { get; set; }

    public string PriceId { get; set; } = string.Empty;

    public string ProcessorId { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public virtual ICollection<PayChargeTaxAmount> TaxAmounts { get; set; } = new List<PayChargeTaxAmount>();

    public int? UnitAmount { get; set; }
}

public class PayChargeRefund
{
    public int Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Description { get; set; } = string.Empty;

    public string ProcessorId { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}

public class PayChargeTaxAmount
{
    public int Amount { get; set; }

    public string Description { get; set; } = string.Empty;
}

public class PayChargeTotalDiscount
{
    public int Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public string DiscountId { get; set; } = string.Empty;
}