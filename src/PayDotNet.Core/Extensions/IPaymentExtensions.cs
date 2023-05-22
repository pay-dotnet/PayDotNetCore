namespace PayDotNet.Core;

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
}