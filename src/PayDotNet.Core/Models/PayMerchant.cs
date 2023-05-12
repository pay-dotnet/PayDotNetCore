namespace PayDotNet.Core.Models;

public class PayMerchant : Timestamps
{
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    public bool IsOnboardingComplete { get; set; }
}