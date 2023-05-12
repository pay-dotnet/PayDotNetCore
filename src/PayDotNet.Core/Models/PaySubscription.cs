namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: unique(customer, processorId)
/// </summary>
public class PaySubscription : Timestamps
{
    public string CustomerId { get; set; }

    public string Name { get; set; }

    // public string Processor { get; set; } Can be found in the Customer.

    public string ProcessorId { get; set; }

    public string ProcessorPlan { get; set; }

    public int Quantity { get; set; } = 1;

    public PaySubscriptionStatus Status { get; set; }

    public DateTime? CurrentPeriodStart { get; set; }

    public DateTime? CurrentPeriodEnd { get; set; }

    public DateTime? TrailEndsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public bool IsMetered { get; set; }

    public PaySubscriptionPauseBehaviour? PauseBehaviour { get; set; }

    public DateTime? PauseStartsAt { get; set; }

    public DateTime? PauseResumesAt { get; set; }

    // TÖDO: 8,2
    public decimal? ApplicationFeePercent { get; set; }

    public virtual ICollection<PayCharge> Charges { get; init; } = new List<PayCharge>();

    public virtual ICollection<PaySubscriptionItem> SubscriptionItems { get; set; } = new List<PaySubscriptionItem>();

    public bool IsOnTrial()
    {
        return TrailEndsAt.HasValue && TrailEndsAt.Value > DateTime.UtcNow;
    }

    public bool IsTrialEnded()
    {
        return TrailEndsAt.HasValue && TrailEndsAt.Value <= DateTime.UtcNow;
    }

    public bool IsIncomplete()
    {
        return Status == PaySubscriptionStatus.Incomplete;
    }

    public void CancelNow()
    {
        Status = PaySubscriptionStatus.Cancelled;
        EndsAt = DateTime.UtcNow;
    }
}

public class PaySubscriptionItem
{
    public string Id { get; set; } = default!;

    public PaySubscriptionItemPrice Price { get; set; } = default!;

    public long Quantity { get; set; }
}

public class PaySubscriptionItemPrice
{
    public string Id { get; set; }
}