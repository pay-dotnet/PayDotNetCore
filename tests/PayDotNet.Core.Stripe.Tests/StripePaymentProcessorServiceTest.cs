using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Tests;

public class StripePaymentProcessorServiceTest : TestBase<StripePaymentProcessorService>
{
    public StripePaymentProcessorServiceTest()
    {
        StripeConfiguration.ApiKey = "sk_test_fake";
    }

    [Fact]
    public async Task create_customer()
    {
        var customer = await SystemUnderTest.CreateCustomerAsync("dotnetfromthemountain@gmail.com", new());
        customer.Should().NotBeNull();
    }

    [Fact]
    public async Task create_subscription_for_customer()
    {
        var stripeCustomer = await SystemUnderTest.CreateCustomerAsync("dotnetfromthemountain@gmail.com", new());
        stripeCustomer.Should().NotBeNull();

        var customer = new PayCustomer()
        {
            ProcessorId = stripeCustomer.Id,
        };

        var stripeSubscription = await SystemUnderTest.CreateSubscriptionAsync(customer, "price_1MyDj2JUAL06t0UNphFwwc6l", new());
        stripeSubscription.Should().NotBeNull();
    }
}