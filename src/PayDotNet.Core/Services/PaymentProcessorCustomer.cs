namespace PayDotNet.Core.Services;

public record PaymentProcessorCustomer(string Id, Dictionary<string, string> Attributes);
