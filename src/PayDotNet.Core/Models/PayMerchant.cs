namespace PayDotNet.Core.Models;

public class PayMerchant : Timestamps
{
    public bool IsDefault { get; set; }

    public bool IsOnboardingComplete { get; set; }

    public string Processor { get; set; }

    public string ProcessorId { get; set; }
}