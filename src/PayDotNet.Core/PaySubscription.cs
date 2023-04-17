using PayDotNet.Core.Abstraction;

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
    public PayStatus Status { get; set; }

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


public interface IPayment
{
    string Id { get; }

    long Amount { get; }

    string ClientSecret { get; }

    string Currency { get; }

    string CustomerId { get; }

    string Status { get; }

    bool RequiresPaymentMethod();

    bool RequiresAction();

    bool IsCanceled();

    bool IsSucceeded();
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
}