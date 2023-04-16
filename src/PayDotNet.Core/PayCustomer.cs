namespace PayDotNet.Core;

/// <summary>
/// TODO: required id+email
/// TODO: unique (processor, processorId)
/// </summary>
public class PayCustomer
{
    public string Id { get; set; }

    public string Email { get; set; }

    public ICollection<PayCharges> Charges { get; set; }

    public ICollection<PaySubscription> Subscriptions { get; set; }

    public ICollection<PayPaymentMethod> PaymentMethods { get; set; }

    // Stripe, FakeProcessor, Braintree, etc.
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    // TODO: JSON
    public string Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime DeletedAt { get; set; }

    public bool HasProcessorId()
    {
        return !string.IsNullOrEmpty(ProcessorId);
    }
}