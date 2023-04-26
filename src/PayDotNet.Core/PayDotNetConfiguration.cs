namespace PayDotNet.Core;

public class PayDotNetConfiguration
{
    public string DefaultProductName { get; set; } = "default";
    public string DefaultPlanName { get; set; } = "default";

    public PayDotNetStripeConfiguration Stripe { get; set; } = new();
}

public class PayDotNetStripeConfiguration
{
    public string? EndpointSecret { get; set; }
}