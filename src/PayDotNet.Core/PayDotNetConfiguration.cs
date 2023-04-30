namespace PayDotNet.Core;

public class PayDotNetConfiguration
{
    public string DefaultProductName { get; set; } = "default";
    public string DefaultPlanName { get; set; } = "default";

    public PayDotNetStripeConfiguration Stripe { get; set; } = new();
}

public class PayDotNetStripeConfiguration
{
    public static readonly string DefaultPaymentBehaviour = "default_incomplete";

    public string? ApiKey { get; set; }

    public string? EndpointSecret { get; set; }

    public string? PublicKey { get; set; }

    public string PaymentBehaviour { get; set; } = DefaultPaymentBehaviour;
}