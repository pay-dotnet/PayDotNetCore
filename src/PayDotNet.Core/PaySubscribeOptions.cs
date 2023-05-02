namespace PayDotNet.Core;

public record PaySubscribeOptions(string Name, List<PaySubscribeOptionsItem> Items)
{
    public PaySubscribeOptions(string Name, string PriceId)
        : this(Name, new List<PaySubscribeOptionsItem>() { new(PriceId) })
    {
    }
}

public record PayCustomerOptions(string ProcessorName, string? PaymentMethodId = null, bool AllowFake = false);

public record PayCancelSubscriptionOptions(CancellationReason Feedback = CancellationReason.Other, string? Comment = null, bool Prorate = false, bool CancelAtEndPeriod = true);

public enum CancellationReason
{
    /// <summary>
    /// Other reason
    /// </summary>
    Other = 0,

    /// <summary>
    /// It’s too expensive
    /// </summary>
    TooExpansive,

    /// <summary>
    /// Some features are missing
    /// </summary>
    MissingFeatures,

    /// <summary>
    /// I’m switching to a different service
    /// </summary>
    SwitchedService,

    /// <summary>
    /// I don’t use the service enough
    /// </summary>
    Unused,

    /// <summary>
    /// Customer service was less than expected
    /// </summary>
    CustomerService,

    /// <summary>
    /// Ease of use was less than expected
    /// </summary>
    TooComplex,

    /// <summary>
    /// Quality was less than expected
    /// </summary>
    LowQuality,
}