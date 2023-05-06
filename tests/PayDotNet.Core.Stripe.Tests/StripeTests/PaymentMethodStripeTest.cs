using Microsoft.Extensions.Options;
using PayDotNet.Core.Models;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public class PaymentMethodStripeTest : StripeTestBase<StripePaymentProcessorService>
{
    public PaymentMethodStripeTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task stripe_attach_payment_method_to_customer()
    {
        string customerProcessorId = await SystemUnderTest.CreateCustomerAsync(new PayCustomer()
        {
            Email = "johndoe@email.com"
        });
        customerProcessorId.Should().NotBeNull();
        PayCustomer payCustomer = new()
        {
            ProcessorId = customerProcessorId,
        };

        // Simulate creation of payment method through another form.
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        // Attach payment method.
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, new(paymentMethod.Id, IsDefault: true));
        payCustomer.PaymentMethods.Add(new()
        {
            ProcessorId = paymentMethod.Id,
            IsDefault = true,
        });

        payPaymentMethod.ProcessorId.Should().Be(paymentMethod.Id);
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(StripeConfiguration.StripeClient, options);
    }
}