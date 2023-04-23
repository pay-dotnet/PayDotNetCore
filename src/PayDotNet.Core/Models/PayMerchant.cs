namespace PayDotNet.Core.Models;

public class PayMerchant
{
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}