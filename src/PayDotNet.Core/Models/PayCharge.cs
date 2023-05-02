namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: unique (customerid, processorid)
/// </summary>
public class PayCharge
{
    public virtual PayCustomer Customer { get; set; }

    public string CustomerId { get; set; }

    public virtual PaySubscription? Subscription { get; set; }

    public string? SubscriptionId { get; set; }

    public string ProccesorId { get; set; }

    public string Brand { get; set; }

    public int Amount { get; set; }

    public string? Currency { get; set; }

    public int? ApplicationFeeAmount { get; set; }

    public int? AmountRefunded { get; set; }

    public int AmountCaptured { get; set; }

    public int ExpirationMonth { get; set; }

    public int ExpirationYear { get; set; }

    public string PaymentIntentId { get; set; }

    public string PaymentMethodType { get; set; }

    public string ReceiptUrl { get; set; }

    public string Last4 { get; set; }

    public Dictionary<string, string> Metadata { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public PayStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PayChargeLineItem> LineItems { get; set; } = new List<PayChargeLineItem>();

    public virtual ICollection<PayChargeDiscount> Discounts { get; set; } = new List<PayChargeDiscount>();

    public virtual ICollection<PayChargeTaxAmount> TaxAmounts { get; set; } = new List<PayChargeTaxAmount>();

    public virtual ICollection<PayChargeRefund> Refunds { get; set; } = new List<PayChargeRefund>();
    public DateTime PeriodEnd { get; set; }
    public DateTime PeriodStart { get; set; }
    public string? InvoiceId { get; set; }
    public int Subtotal { get; set; }
    public int? Tax { get; set; }
}