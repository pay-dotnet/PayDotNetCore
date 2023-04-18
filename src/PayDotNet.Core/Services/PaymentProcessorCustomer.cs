namespace PayDotNet.Core.Abstraction;

public record PaymentProcessorCustomer(string Id, Dictionary<string, string> Attributes);
