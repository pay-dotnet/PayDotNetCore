namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: unique(customer, processorId)
/// </summary>
public class PaySubscription
{
    public int Id { get; set; }

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

    public Dictionary<string, string> Metadata { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

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

public static class IPaymentExtensions
{
    public static void Validate(this IPayment payment)
    {
        if (payment.RequiresPaymentMethod())
        {
            throw new InvalidPaymentPayDotNetException(payment);
        }
        if (payment.RequiresAction())
        {
            throw new ActionRequiredPayDotNetException(payment);
        }
    }

    public static bool IsCanceled(this IPayment payment)
    {
        return payment.Status == "canceled";
    }

    public static bool IsSucceeded(this IPayment payment)
    {
        return payment.Status == "succeeded";
    }

    public static bool RequiresAction(this IPayment payment)
    {
        return payment.Status == "requires_action";
    }

    public static bool RequiresPaymentMethod(this IPayment payment)
    {
        return payment.Status == "requires_payment_method";
    }
}