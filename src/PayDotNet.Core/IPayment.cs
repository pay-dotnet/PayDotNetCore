namespace PayDotNet.Core;

public interface IPayment
{
    long Amount { get; }

    string ClientSecret { get; }

    string Currency { get; }

    string CustomerId { get; }

    string Id { get; }

    string Mode { get; }

    PayStatus Status { get; }
}

// TODO: refactor away.
public static class IPaymentExtensions
{
    public static bool IsCanceled(this IPayment payment)
    {
        return payment.Status == PayStatus.Canceled;
    }

    public static bool IsSucceeded(this IPayment payment)
    {
        return payment.Status == PayStatus.Succeeded;
    }

    public static bool RequiresAction(this IPayment payment)
    {
        return payment.Status == PayStatus.RequiresAction;
    }

    public static bool RequiresPaymentMethod(this IPayment payment)
    {
        return payment.Status == PayStatus.RequiresPaymentMethod;
    }

    public static void Validate(this IPayment payment)
    {
        if (payment.RequiresPaymentMethod())
        {
            throw new InvalidPaymentPayDotNetException(payment);
        }
        if (payment.RequiresAction())
        {
            throw new ActionRequiredPayDotNetException(payment);
        }
    }
}