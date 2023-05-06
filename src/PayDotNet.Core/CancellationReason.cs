namespace PayDotNet.Core;

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