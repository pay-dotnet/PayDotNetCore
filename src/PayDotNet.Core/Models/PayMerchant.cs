namespace PayDotNet.Core.Models;

public class PayMerchant
{
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    // TODO: JSON
    public string Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
