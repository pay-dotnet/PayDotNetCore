namespace PayDotNet.Core.Abstraction;
public interface ICustomerManager
{
    Task<PayCustomer> CreateAsync(string processor, string id, string email);

    Task<PayCustomer?> FindByIdAsync(string processor, string id);

    Task<PayCustomer?> FindByEmailAsync(string processor, string email);
}

public record PaymentProcessorCustomer(string Id, Dictionary<string, string> Attributes);
public abstract record PaymentProcessorSubscription(string Id, Dictionary<string, object?> Attributes)
{
    public abstract DateTime? GetTrialEndDate();
};


[Serializable]
public class PayDotNetException : Exception
{
    public PayDotNetException() { }
    public PayDotNetException(string message) : base(message) { }
    public PayDotNetException(string message, Exception inner) : base(message, inner) { }
    protected PayDotNetException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}