namespace PayDotNet.Stores;

public class PayPaymentMethod : Timestamps
{
    public string CustomerId { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public string ProcessorId { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}