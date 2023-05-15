using System.Text;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Stripe.Client;

internal static class StripeStatusMapper
{
    public static PaySubscriptionPauseBehaviour? GetPauseBehaviour(string? behavior)
    {
        if (string.IsNullOrEmpty(behavior))
        {
            return null;
        }

        switch (behavior)
        {
            case "void":
                return PaySubscriptionPauseBehaviour.Void;

            case "mark_uncollectible":
                return PaySubscriptionPauseBehaviour.MarkUncollectible;

            case "keep_as_draft":
                return PaySubscriptionPauseBehaviour.KeepAsDraft;

            default:
                return PaySubscriptionPauseBehaviour.None;
        }
    }

    public static PayStatus GetPayStatus(string status) => status switch
    {
        "requires_payment_method" => PayStatus.RequiresPaymentMethod,
        "requires_confirmation" => PayStatus.RequiresConfirmation,
        "requires_action" => PayStatus.RequiresAction,
        "requires_capture" => PayStatus.RequiresCapture,
        "succeeded" => PayStatus.Succeeded,
        "processing" => PayStatus.Processing,
        "canceled" => PayStatus.Canceled,
        _ => PayStatus.None,
    };

    public static PaySubscriptionStatus GetSubscriptionStatus(string status) => status switch
    {
        "incomplete" => PaySubscriptionStatus.Incomplete,
        "incomplete_expired" => PaySubscriptionStatus.IncompleteExpired,
        "trialing" => PaySubscriptionStatus.Trialing,
        "active" => PaySubscriptionStatus.Active,
        "past_due" => PaySubscriptionStatus.PastDue,
        "canceled" => PaySubscriptionStatus.Cancelled,
        "unpaid" => PaySubscriptionStatus.Unpaid,
        _ => PaySubscriptionStatus.None,
    };

    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder result = new StringBuilder();
        bool isFirst = true;

        foreach (char c in input)
        {
            if (Char.IsWhiteSpace(c))
            {
                continue;
            }

            if (Char.IsUpper(c))
            {
                if (!isFirst)
                {
                    result.Append("_");
                }

                result.Append(Char.ToLower(c));
            }
            else
            {
                result.Append(c);
            }

            isFirst = false;
        }

        return result.ToString();
    }
}