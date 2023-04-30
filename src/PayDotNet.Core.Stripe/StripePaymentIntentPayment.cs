using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe;

public record StripePaymentIntentPayment(PaymentIntent Intent) : IPayment
{
    public string Id => Intent.Id;

    public long Amount => Intent.Amount;

    public string ClientSecret => Intent.ClientSecret;

    public string Currency => Intent.Currency;

    public string CustomerId => Intent.CustomerId;

    public string Status => Intent.Status;

    public string Mode => "payment";
}