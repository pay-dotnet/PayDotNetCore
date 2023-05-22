using System.Runtime.Serialization;

namespace PayDotNet.Core;

[Serializable]
public class ActionRequiredPayDotNetException : PayDotNetException
{
    public ActionRequiredPayDotNetException(IPayment payment)
    {
        Payment = payment;
    }

    public ActionRequiredPayDotNetException(IPayment payment, string message)
        : base(message)
    {
        Payment = payment;
    }

    public ActionRequiredPayDotNetException(IPayment payment, string message, Exception inner)
        : base(message, inner)
    {
        Payment = payment;
    }

    protected ActionRequiredPayDotNetException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
        Payment = (IPayment)info.GetValue(nameof(Payment), typeof(IPayment))!;
    }

    public IPayment Payment { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);
        info.AddValue(nameof(Payment), Payment);
        base.GetObjectData(info, context);
    }
}