namespace PayDotNet.Stores;

public class PaySubscription : Timestamps
{
    public decimal? ApplicationFeePercent { get; set; }

    public virtual ICollection<PayCharge> Charges { get; set; } = new List<PayCharge>();

    public DateTime? CurrentPeriodEnd { get; set; }

    public DateTime? CurrentPeriodStart { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public DateTime? EndsAt { get; set; }

    public bool IsMetered { get; set; }

    public string Name { get; set; } = string.Empty;

    public PaySubscriptionPauseBehaviour? PauseBehaviour { get; set; }

    public DateTime? PauseResumesAt { get; set; }

    public DateTime? PauseStartsAt { get; set; }

    public string ProcessorId { get; set; } = string.Empty;

    public string ProcessorPlan { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public PaySubscriptionStatus Status { get; set; }

    public string? SubscriptionId { get; set; }

    public virtual ICollection<PaySubscriptionItem> SubscriptionItems { get; set; } = new List<PaySubscriptionItem>();

    public DateTime? TrailEndsAt { get; set; }

    public void CancelNow()
    {
        Status = PaySubscriptionStatus.Cancelled;
        EndsAt = DateTime.UtcNow;
    }

    public bool IsIncomplete()
    {
        return Status == PaySubscriptionStatus.Incomplete;
    }

    public bool IsOnTrial()
    {
        return TrailEndsAt.HasValue && TrailEndsAt.Value > DateTime.UtcNow;
    }

    public bool IsTrialEnded()
    {
        return TrailEndsAt.HasValue && TrailEndsAt.Value <= DateTime.UtcNow;
    }
}

public class PaySubscriptionItem
{
    public string Id { get; set; } = default!;

    public PaySubscriptionItemPrice Price { get; set; } = default!;

    public int Quantity { get; set; }
}

public class PaySubscriptionItemPrice
{
    public string Id { get; set; } = string.Empty;
}