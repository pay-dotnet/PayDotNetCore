using Microsoft.Extensions.Options;
using PayDotNet.Core.Models;
using Scotch;
using Stripe;

namespace PayDotNet.Core.Stripe.Tests;

public class StripePaymentProcessorServiceTest : TestBase<StripePaymentProcessorService>
{
    public StripePaymentProcessorServiceTest()
    {
        HttpClient httpClient = HttpClients.NewHttpClient("../../../ScotchCassettes/recordings.json", ScotchMode.Recording);
        StripeConfiguration.StripeClient = new StripeClient("sk_test_51IQViRJUAL06t0UNemg2YVbfQ150HFQm7MdQmBjznqR0lUD9QR65dTfBHIoP11rBdU9I8QYmmvpzN3Y6bgG4XGQ500BPIpaZwU", httpClient: new SystemNetHttpClient(httpClient));
    }

    [Fact]
    public async Task create_customer()
    {
        string customerProcessorId = await SystemUnderTest.CreateCustomerAsync(new PayCustomer()
        {
            Email = "johndoe@email.com"
        });
        customerProcessorId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task create_subscription_for_customer()
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

        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, "price_1MyDj2JUAL06t0UNphFwwc6l"));
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeFalse();
        result.Payment.RequiresPaymentMethod().Should().BeTrue();
    }

    [Fact]
    public async Task create_subscription_for_customer_with_payment_method()
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

        // Create payment method
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(new()
        {
            Type = "card",
            Card = new()
            {
                Number = StripeCards.Visa,
                ExpMonth = 12,
                ExpYear = 2030,
                Cvc = "424",
            }
        });

        // Attach payment method.
        await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);
        payCustomer.PaymentMethods.Add(new()
        {
            ProcessorId = paymentMethod.Id,
            IsDefault = true,
        });

        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, "price_1MyDj2JUAL06t0UNphFwwc6l"));
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeTrue();
        result.Payment.RequiresPaymentMethod().Should().BeFalse();
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(options);
    }
}

public static class StripeCards
{
    public static readonly string Visa = "4242424242424242";
}