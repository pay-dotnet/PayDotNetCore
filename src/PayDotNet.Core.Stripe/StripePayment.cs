using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe;

public record StripePayment(PaymentIntent Intent) : IPayment
{
    public string Id => Intent.Id;

    public long Amount => Intent.Amount;

    public string ClientSecret => Intent.ClientSecret;

    public string Currency => Intent.Currency;

    public string CustomerId => Intent.CustomerId;

    public string Status => Intent.Status;

    public bool IsCanceled()
    {
        return Status == "canceled";
    }

    public bool IsSucceeded()
    {
        return Status == "succeeded";
    }

    public bool RequiresAction()
    {
        return Status == "requires_action";
    }

    public bool RequiresPaymentMethod()
    {
        return Status == "requires_payment_method";
    }
}