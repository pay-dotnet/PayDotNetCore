using System.Runtime.Serialization;

namespace PayDotNet.Core;

[Serializable]
public class InvalidPaymentPayDotNetException : PayDotNetException
{
    public InvalidPaymentPayDotNetException(IPayment payment)
    {
        Payment = payment;
    }

    public InvalidPaymentPayDotNetException(IPayment payment, string message)
        : base(message)
    {
        Payment = payment;
    }

    public InvalidPaymentPayDotNetException(IPayment payment, string message, Exception inner)
        : base(message, inner)
    {
        Payment = payment;
    }

    protected InvalidPaymentPayDotNetException(
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