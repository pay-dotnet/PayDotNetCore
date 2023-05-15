using PayDotNet.Core.Models;

namespace PayDotNet.Core;

[Serializable]
public class InvalidPaymentPayDotNetException : PayDotNetException
{
    public InvalidPaymentPayDotNetException(IPayment payment)
    {
        Payment = payment;
    }

    public InvalidPaymentPayDotNetException(string message) : base(message)
    {
    }

    public InvalidPaymentPayDotNetException(string message, Exception inner) : base(message, inner)
    {
    }

    protected InvalidPaymentPayDotNetException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public IPayment Payment { get; }
}