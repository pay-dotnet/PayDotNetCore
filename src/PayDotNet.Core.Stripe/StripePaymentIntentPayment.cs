using PayDotNet.Core.Stripe.Client;
using Stripe;

namespace PayDotNet.Core.Stripe;

public record StripePaymentIntentPayment(PaymentIntent Intent) : IPayment
{
    public string Id => Intent.Id;

    public long Amount => Intent.Amount;

    public string ClientSecret => Intent.ClientSecret;

    public string Currency => Intent.Currency;

    public string CustomerId => Intent.CustomerId;

    public PayStatus Status => StripeStatusMapper.GetPayStatus(Intent.Status);

    public string Mode => "payment";
}