using Microsoft.Extensions.Options;
using PayDotNet.Core.Models;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public class CheckoutStripeTest : StripeTestBase<StripePaymentProcessorService>
{
    public CheckoutStripeTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    public static PayCheckoutOptions CheckoutOptions = new(LineItems: new()
    {
        new PayCheckoutLineItem(PriceData: new("T-shirt", 30_00, "eur")),
        new PayCheckoutLineItem(PriceData: new("Jeans", 45_00, "eur")),
        new PayCheckoutLineItem(PriceData: new("Shoes", 25_00, "eur")),
        new PayCheckoutLineItem(PriceData: new("Socks", 5_00, "eur")),
    });

    [Fact]
    public async Task stripe_can_checkout_payment()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);

        // Act
        PayCheckoutResult result = await SystemUnderTest.CheckoutAsync(payCustomer, CheckoutOptions);

        // Assert
        result.SuccessUrl.ToString().Should().Contain("{CHECKOUT_SESSION_ID}");
        result.Mode.Should().Be("payment");
    }

    [Fact]
    public async Task stripe_can_checkout_setup()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);

        // Act
        PayCheckoutResult result = await SystemUnderTest.CheckoutAsync(payCustomer, new("setup"));

        // Assert
        result.SuccessUrl.ToString().Should().Contain("{CHECKOUT_SESSION_ID}");
        result.Mode.Should().Be("setup");
    }

    [Fact]
    public async Task stripe_can_checkout_subscription()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);

        // Act
        PayCheckoutResult result = await SystemUnderTest.CheckoutAsync(payCustomer, new(LineItems: new()
            {
                new PayCheckoutLineItem(StripeData.BasicSubscription)
            },
            Mode: "subscription"));

        // Assert
        result.SuccessUrl.ToString().Should().Contain("{CHECKOUT_SESSION_ID}");
        result.Mode.Should().Be("subscription");
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(StripeConfiguration.StripeClient, options);
    }
}

public class ChargesStripeTest : StripeTestBase<StripePaymentProcessorService>
{
    public ChargesStripeTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task stripe_can_charge_customer()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);

        // Act
        PayChargeResult payChargeResult =
            await SystemUnderTest.ChargeAsync(payCustomer, new PayChargeOptions(29_00, "eur", PaymentMethodId: payPaymentMethod.ProcessorId));

        // Assert
        payChargeResult.Payment.IsSucceeded().Should().BeTrue();
    }

    [Fact]
    public async Task stripe_can_charge_customer_raises_payment_declined()
    {
        // Arrange
        var payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.DeclineAfterAttaching);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);

        // Act
        Func<Task> action = () =>
            SystemUnderTest.ChargeAsync(payCustomer, new PayChargeOptions(29_00, "eur", PaymentMethodId: payPaymentMethod.ProcessorId));

        // Assert
        await action.Should().ThrowAsync<PayDotNetStripeException>();
    }

    [Fact]
    public async Task stripe_can_charge_customer_required_action_for_sca()
    {
        // Arrange
        var payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.SCA);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);

        // Act
        PayChargeResult result =
            await SystemUnderTest.ChargeAsync(payCustomer, new PayChargeOptions(29_00, "eur", PaymentMethodId: payPaymentMethod.ProcessorId));

        // Assert
        result.PayCharge.Should().BeNull("Because the charge didn't exceed and the payment will indicate an action is required");
        result.Payment.IsSucceeded().Should().BeFalse();
        result.Payment.RequiresAction().Should().BeTrue();
    }

    [Fact]
    public async Task stripe_charge_without_payment_method_should_throw_exception()
    {
        // Arrange
        var payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);

        // Act
        Func<Task> action = () =>
            SystemUnderTest.ChargeAsync(payCustomer, new PayChargeOptions(29_00, "eur"));

        // Assert
        await action.Should().ThrowAsync<PayDotNetStripeException>();
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(StripeConfiguration.StripeClient, options);
    }
}