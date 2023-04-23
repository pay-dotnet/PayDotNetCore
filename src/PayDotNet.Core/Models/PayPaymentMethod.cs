namespace PayDotNet.Core.Models;

/// TODO: unique(customer, processorId)
public class PayPaymentMethod
{
    public virtual PayCustomer Customer { get; set; }

    public string CustomerId { get; set; }

    public string ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    public string Type { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}