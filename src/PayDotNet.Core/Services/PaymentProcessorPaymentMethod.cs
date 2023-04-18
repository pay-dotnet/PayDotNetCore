namespace PayDotNet.Core.Abstraction;

public record PaymentProcessorPaymentMethod(string Id, string Type, bool IsDefault);
