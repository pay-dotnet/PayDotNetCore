namespace PayDotNet.Core.Models;

public class PayCustomer : Timestamps
{
    public virtual ICollection<PayCharge> Charges { get; init; } = new List<PayCharge>();

    public PayPaymentMethod? DefaultPaymentMethod => PaymentMethods.FirstOrDefault(p => p.IsDefault);

    public DateTime DeletedAt { get; set; }

    public string Email { get; set; }

    // TODO: generic key.
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsDefault { get; set; }

    public virtual ICollection<PayPaymentMethod> PaymentMethods { get; init; } = new List<PayPaymentMethod>();

    // Stripe, FakeProcessor, Braintree, etc.
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    public virtual ICollection<PaySubscription> Subscriptions { get; init; } = new List<PaySubscription>();

    public bool HasProcessorId()
    {
        return !string.IsNullOrEmpty(ProcessorId);
    }
}