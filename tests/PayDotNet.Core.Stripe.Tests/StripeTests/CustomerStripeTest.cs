using Microsoft.Extensions.Options;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public class CustomerStripeTest : StripeTestBase<StripePaymentProcessorService>
{
    public CustomerStripeTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task stripe_create_customer()
    {
        var result = await SystemUnderTest.CreateCustomerAsync(new PayCustomer()
        {
            Email = "johndoe@email.com"
        });
        result.ProcessorId.Should().NotBeNullOrEmpty();
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(StripeConfiguration.StripeClient, options);
    }
}