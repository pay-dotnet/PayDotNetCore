namespace PayDotNet.Core.Abstraction;
public interface ICustomerManager
{
    Task<PayCustomer> CreateIfNotExistsAsync(string id, string email, string processorName);

    Task UpdateAsync(PayCustomer customer);

    Task<PayCustomer?> FindByIdAsync(string id, string processorName);

    Task<PayCustomer?> FindByEmailAsync(string email, string processorName);
}

public interface IPaymentMethodManager
{
    Task<PayPaymentMethod> CreateAsync(PayCustomer customer, string paymentMethodId, bool isDefault = false);
}

public record PaymentProcessorCustomer(string Id, Dictionary<string, string> Attributes);

public record PaymentProcessorPaymentMethod(string Id, string Type, bool IsDefault);

public record PaymentProcessorSubscription(string Id, string CustomerId, Dictionary<string, object?> Attributes, IPayment Payment)
{
    public virtual DateTime? GetTrialEndDate() => null;
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


[Serializable]
public class InvalidPaymentPayDotNetException : PayDotNetException
{
    public InvalidPaymentPayDotNetException(IPayment payment)
    {
        Payment = payment;
    }

    public InvalidPaymentPayDotNetException(string message) : base(message) { }
    public InvalidPaymentPayDotNetException(string message, Exception inner) : base(message, inner) { }
    protected InvalidPaymentPayDotNetException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public IPayment Payment { get; }
}

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