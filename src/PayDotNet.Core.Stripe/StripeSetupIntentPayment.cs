using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe;

public record StripeSetupIntentPayment(SetupIntent Intent) : IPayment
{
    public string Id => Intent.Id;

    public long Amount => 0;

    public string ClientSecret => Intent.ClientSecret;

    public string Currency => "";

    public string CustomerId => Intent.CustomerId;

    public string Status => Intent.Status;

    public string Mode => "setup";
}