namespace PayDotNet.Core;

/// <summary>
/// TODO: unique (customerid, processorid)
/// </summary>
public class PayCharges
{
    public virtual PayCustomer Customer { get; set; }

    public string CustomerId { get; set; }

    public virtual PaySubscription? Subscription { get; set; }

    public string? SubscriptionId { get; set; }

    public string ProccesorId { get; set; }

    public int Amount { get; set; }

    public string? Currency { get; set; }

    public int? ApplicationFeeAmount { get; set; }

    public int? AmountRefunded { get; set; }

    // TODO: JSON
    public string Metadata { get; set; }

    // TODO: JSON
    public string Data { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
