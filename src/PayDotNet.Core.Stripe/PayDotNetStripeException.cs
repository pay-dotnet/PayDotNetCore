namespace PayDotNet.Core;

[Serializable]
public class PayDotNetStripeException : Exception
{
    public const string DefaultMessage = "Stripe API threw an exception. See inner exception for details";

    public PayDotNetStripeException()
    { }

    public PayDotNetStripeException(string message) : base(message)
    {
    }

    public PayDotNetStripeException(string message, Exception inner) : base(message, inner)
    {
    }

    protected PayDotNetStripeException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}