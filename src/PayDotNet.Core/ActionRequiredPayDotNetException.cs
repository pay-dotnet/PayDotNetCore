using PayDotNet.Core.Models;

namespace PayDotNet.Core;

[Serializable]
public class ActionRequiredPayDotNetException : PayDotNetException
{
    public ActionRequiredPayDotNetException(IPayment payment)
    {
        Payment = payment;
    }

    public ActionRequiredPayDotNetException(string message) : base(message) { }
    public ActionRequiredPayDotNetException(string message, Exception inner) : base(message, inner) { }
    protected ActionRequiredPayDotNetException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public IPayment Payment { get; }
}