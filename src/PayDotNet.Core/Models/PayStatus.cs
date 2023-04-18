namespace PayDotNet.Core.Models;

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
