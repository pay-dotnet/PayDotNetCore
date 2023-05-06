using PayDotNet.Core.Models;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public class StripeTestBase<TSystemUnderTest> : TestBase<TSystemUnderTest>
{
    protected StripeTestBase(ITestOutputHelper testOutputHelper)
    {
        testOutputHelper.SetupStripeRecording();
    }

    public PayCustomer NewCustomer => new PayCustomer()
    {
        Email = "dotnetfromthemountain+test@gmail.com"
    };

    public class Subscriptions
    {
        public const string BasicSubscription = "price_1MyDj2JUAL06t0UNphFwwc6l";
        public const string PremiumSubscription = "price_1MyDj2JUAL06t0UNjnmx5za5";
    }

    public class PaymentMethods
    {
        public static PaymentMethodCreateOptions Visa4242 => new()
        {
            Type = "card",
            Card = new()
            {
                Number = "4242424242424242",
                ExpMonth = 12,
                ExpYear = 2030,
                Cvc = "424",
            }
        };

        public static PaymentMethodCreateOptions GenericDeclined => new()
        {
            Type = "card",
            Card = new()
            {
                Number = "4000000000000002",
                ExpMonth = 12,
                ExpYear = 2030,
                Cvc = "111"
            }
        };

        public static PaymentMethodCreateOptions InsufficientFundsDeclined => new()
        {
            Type = "card",
            Card = new()
            {
                Number = "4000000000009995",
                ExpMonth = 12,
                ExpYear = 2030,
                Cvc = "111"
            }
        };

        public static PaymentMethodCreateOptions DeclineAfterAttaching => new()
        {
            Type = "card",
            Card = new()
            {
                Number = "4000000000000341",
                ExpMonth = 12,
                ExpYear = 2030,
                Cvc = "111"
            }
        };

        public static PaymentMethodCreateOptions SCA => new()
        {
            Type = "card",
            Card = new()
            {
                Number = "4000002500003155",
                ExpMonth = 12,
                ExpYear = 2030,
                Cvc = "111"
            }
        };
    }
}