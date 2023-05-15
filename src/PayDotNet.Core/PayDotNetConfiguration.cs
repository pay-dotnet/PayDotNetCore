namespace PayDotNet.Core;

public class PayDotNetConfiguration
{
    public string DefaultPaymentProcessor { get; set; } = PaymentProcessors.Stripe;

    public string DefaultPlanName { get; set; } = "default";

    public string DefaultProductName { get; set; } = "default";

    public string DefaultRefundDescription { get; set; } = "Refund";

    public string RootUrl { get; set; } = "https://localhost:7148";

    public PayDotNetStripeConfiguration Stripe { get; set; } = new();
}

public class PayDotNetStripeConfiguration
{
    public static readonly string DefaultPaymentBehaviour = "allow_incomplete";

    public string? ApiKey { get; set; }

    public string? EndpointSecret { get; set; }

    public string PaymentBehaviour { get; set; } = DefaultPaymentBehaviour;

    public List<string> PaymentMethodTypes { get; set; } = new()
    {
        "card", "ideal", "link"
    };

    public string? PublicKey { get; set; }
}