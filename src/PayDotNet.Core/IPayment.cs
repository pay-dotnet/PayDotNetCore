namespace PayDotNet.Core;

public interface IPayment
{
    long Amount { get; }

    string ClientSecret { get; }

    string Currency { get; }

    string CustomerId { get; }

    string Id { get; }

    string Mode { get; }

    PayStatus Status { get; }
}