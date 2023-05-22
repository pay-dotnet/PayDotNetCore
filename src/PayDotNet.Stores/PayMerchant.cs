namespace PayDotNet.Stores;

public class PayMerchant : Timestamps
{
    public bool IsDefault { get; set; }

    public bool IsOnboardingComplete { get; set; }

    public string Processor { get; set; } = string.Empty;

    public string ProcessorId { get; set; } = string.Empty;
}