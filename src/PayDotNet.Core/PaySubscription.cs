namespace PayDotNet.Core;

/// <summary>
/// TODO: unique(customer, processorId)
/// </summary>
public class PaySubscription
{
    public virtual PayCustomer Customer { get; set; }

    public string CustomerId { get; set; }

    public string Name { get; set; }

    public string ProcessorId { get; set; }

    public string ProcessorPlan { get; set; }

    // TODO: default 1
    public int Quantity { get; set; }

    // TODO: enum?
    public string Status { get; set; }

    public DateTime? CurrentPeriodStart { get; set; }

    public DateTime? CurrentPeriodEnd { get; set; }

    public DateTime? TrailEndsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public bool IsMetered { get; set; }

    public string? PauseBehaviour { get; set; }

    public DateTime? PauseStartsAt { get; set; }

    public DateTime? PauseResumesAt { get; set; }

    // TÖDO: 8,2
    public decimal? ApplicationFeePercent { get; set; }

    // TODO: JSON
    public string Metadata { get; set; }

    // TODO: JSON
    public string Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
