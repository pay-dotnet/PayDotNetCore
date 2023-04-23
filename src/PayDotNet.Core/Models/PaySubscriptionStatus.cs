namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: map stripe vs locally.
/// </summary>

public enum PaySubscriptionStatus
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

public enum PayStatus
{
    None = 0,
    Void,
    Succeeded,
    Warning,
    Error,
}