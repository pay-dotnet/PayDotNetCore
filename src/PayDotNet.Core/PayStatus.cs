namespace PayDotNet.Core;

/// <summary>
/// TODO: map stripe vs locally.
/// </summary>

public enum PayStatus
{
    None = 0,
    Incomplete,
    IncompleteExpired,
    Trialing,
    Active,
    PastDue,
    Cancelled,
    Unpaid,
    Paused
}


public static class PaymentProcessors
{
    public static readonly string Stripe = "stripe";
    public static readonly string Fake = "fake_processor";
}