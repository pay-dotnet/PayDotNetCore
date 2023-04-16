namespace PayDotNet.Core;

/// TODO: unique(customer, processorId)
public class PayPaymentMethod
{
    public virtual PayCustomer Customer { get; set; }

    public string CustomerId { get; set; }

    public string ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    public string Type { get; set; }

    // TODO: JSON
    public string Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
