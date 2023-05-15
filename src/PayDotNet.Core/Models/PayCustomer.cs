namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: required id+email
/// TODO: unique (processor, processorId)
/// </summary>
public class PayCustomer : Timestamps
{
    // TODO: make readonly so you can't add them
    public ICollection<PayCharge> Charges { get; init; } = new List<PayCharge>();

    public PayPaymentMethod? DefaultPaymentMethod => PaymentMethods.FirstOrDefault(p => p.IsDefault);

    public DateTime DeletedAt { get; set; }

    public string Email { get; set; }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsDefault { get; set; }

    // TODO: make readonly so you can't add them
    public ICollection<PayPaymentMethod> PaymentMethods { get; init; } = new List<PayPaymentMethod>();

    // Stripe, FakeProcessor, Braintree, etc.
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    // TODO: make readonly so you can't add them
    public ICollection<PaySubscription> Subscriptions { get; init; } = new List<PaySubscription>();

    public bool HasProcessorId()
    {
        return !string.IsNullOrEmpty(ProcessorId);
    }
}