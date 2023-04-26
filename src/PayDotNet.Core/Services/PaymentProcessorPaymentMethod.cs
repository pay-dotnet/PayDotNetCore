namespace PayDotNet.Core.Services;

public record PaymentProcessorPaymentMethod(string Id, string Type, bool IsDefault);
