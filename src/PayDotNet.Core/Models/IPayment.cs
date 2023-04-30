namespace PayDotNet.Core.Models;

public interface IPayment
{
    string Id { get; }

    long Amount { get; }

    string ClientSecret { get; }

    string Currency { get; }

    string CustomerId { get; }

    string Status { get; }

    string Mode { get; }
}