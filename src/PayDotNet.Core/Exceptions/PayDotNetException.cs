namespace PayDotNet.Core;

[Serializable]
public class PayDotNetException : Exception
{
    public PayDotNetException()
    { }

    public PayDotNetException(string message) : base(message)
    {
    }

    public PayDotNetException(string message, Exception inner) : base(message, inner)
    {
    }

    protected PayDotNetException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
