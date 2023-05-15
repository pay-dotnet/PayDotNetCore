using PayDotNet.Core.Stripe.Client;
using Stripe;

namespace PayDotNet.Core.Stripe;

public record StripeSetupIntentPayment(SetupIntent Intent) : IPayment
{
    public string Id => Intent.Id;

    public long Amount => 0;

    public string ClientSecret => Intent.ClientSecret;

    public string Currency => "";

    public string CustomerId => Intent.CustomerId;

    public PayStatus Status => StripeStatusMapper.GetPayStatus(Intent.Status);

    public string Mode => "setup";
}