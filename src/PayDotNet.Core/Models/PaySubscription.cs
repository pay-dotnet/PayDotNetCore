namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: unique(customer, processorId)
/// </summary>
public record PaySubscriptionResult(PaySubscription PaySubscription, IPayment? Payment);

public class PaySubscription
{
    public virtual PayCustomer Customer { get; set; }

    public string CustomerId { get; set; }

    public string Name { get; set; }

    public string Processor { get; set; }

    public string ProcessorId { get; set; }

    public string ProcessorPlan { get; set; }

    // TODO: default 1
    public int Quantity { get; set; }

    // TODO: enum?
    public PaySubscriptionStatus Status { get; set; }

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

    public Dictionary<string, string> Metadata { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<PayCharge> Charges { get; init; } = new List<PayCharge>();
}

public static class IPaymentExtensions
{
    public static void Validate(this IPayment payment)
    {
        if (payment.RequiresAction())
        {
            throw new ActionRequiredPayDotNetException(payment);
        }
        if (payment.RequiresPaymentMethod())
        {
            throw new InvalidPaymentPayDotNetException(payment);
        }
    }
}