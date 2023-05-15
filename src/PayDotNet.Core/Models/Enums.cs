namespace PayDotNet.Core.Models;

public enum PayStatus
{
    None = 0,
    RequiresPaymentMethod,
    RequiresConfirmation,
    RequiresAction,
    RequiresCapture,
    Processing,
    Canceled,
    Succeeded,
}

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

public enum PaySubscriptionPauseBehaviour
{
    None = 0,
    Void,
    KeepAsDraft,
    MarkUncollectible
}