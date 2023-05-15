namespace PayDotNet.Core;

public static class PaymentProcessors
{
    public static readonly string Fake = "fake_processor";
    public static readonly string Stripe = "stripe";
}

public static class PayMetadata
{
    public static class Fields
    {
        public static readonly string PaySubscriptionName = "pay_name";
    }
}