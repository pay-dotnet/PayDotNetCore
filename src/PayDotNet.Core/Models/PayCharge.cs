namespace PayDotNet.Core.Models;

public class PayCharge : Timestamps
{
    public int Amount { get; set; }

    public int? AmountRefunded { get; set; }

    public int? ApplicationFeeAmount { get; set; }

    public string Currency { get; set; }

    public string CustomerId { get; set; }

    public string ProcessorId { get; set; }

    public string? SubscriptionId { get; set; }

    #region Additional Properties

    public int AmountCaptured { get; set; }

    public string? Bank { get; set; }

    public string? Brand { get; set; }

    public virtual ICollection<string> Discounts { get; set; } = new List<string>();

    public string? ExpirationMonth { get; set; }

    public string? ExpirationYear { get; set; }

    public string? InvoiceId { get; set; }

    public string? Last4 { get; set; }

    public virtual ICollection<PayChargeLineItem> LineItems { get; set; } = new List<PayChargeLineItem>();

    public string PaymentIntentId { get; set; }

    public string PaymentMethodType { get; set; }

    public DateTime PeriodEnd { get; set; }

    public DateTime PeriodStart { get; set; }

    public string ReceiptUrl { get; set; }

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

    public string Description { get; set; }

    public virtual ICollection<string> Discounts { get; set; } = new List<string>();

    public bool IsProration { get; set; }

    public DateTime PeriodEnd { get; set; }

    public DateTime PeriodStart { get; set; }

    public string PriceId { get; set; }

    public string ProcessorId { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<PayChargeTaxAmount> TaxAmounts { get; set; }

    public int? UnitAmount { get; set; }
}

public class PayChargeRefund
{
    public int Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Description { get; set; }

    public string ProcessorId { get; set; }

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
    public int Amount { get; set; }

    public string Description { get; set; }

    public string DiscountId { get; set; }
}