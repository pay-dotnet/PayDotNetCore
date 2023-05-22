using Microsoft.Extensions.Options;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public class SubscriptionStripeTest : StripeTestBase<StripePaymentProcessorService>
{
    public SubscriptionStripeTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task stripe_can_cancel_subscription()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);
        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, new(paymentMethod.Id, IsDefault: true));
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act #1
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, Subscriptions.BasicSubscription));
        await SystemUnderTest.CancelAsync(payCustomer, result.PaySubscription, new(CancellationReason.Other, "test"));
        result = await SystemUnderTest.GetSubscriptionAsync(payCustomer, result.PaySubscription.ProcessorId);

        // Assert
        result.Should().NotBeNull();
        PaySubscription paySubscription = result.PaySubscription;
        paySubscription.EndsAt.HasValue.Should().BeTrue();
    }

    [Fact]
    public async Task stripe_can_create_subscription()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);
        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, new(paymentMethod.Id, IsDefault: true));
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, Subscriptions.BasicSubscription));

        // Assert
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeTrue();
        result.Payment.RequiresPaymentMethod().Should().BeFalse();

        PaySubscription paySubscription = result.PaySubscription;
        paySubscription.Charges.Should().NotBeEmpty();
        paySubscription.Charges.FirstOrDefault().Amount.Should().BeGreaterThanOrEqualTo(249_00);
    }

    [Fact]
    public async Task stripe_can_create_subscription_also_saves_initial_charge()
    {
        // Assert
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, new(paymentMethod.Id, IsDefault: true));
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, Subscriptions.BasicSubscription));

        // Assert
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeTrue();
        result.Payment.RequiresPaymentMethod().Should().BeFalse();

        PaySubscription paySubscription = result.PaySubscription;
        paySubscription.Charges.Should().NotBeEmpty();
        paySubscription.Charges.FirstOrDefault().Amount.Should().BeGreaterThanOrEqualTo(249_00);
    }

    [Fact]
    public async Task stripe_can_retrieve_subscription()
    {
        // Arrange
        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;

        // Act
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, Subscriptions.BasicSubscription));

        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeFalse();
        result.Payment.RequiresPaymentMethod().Should().BeTrue();
        result.PaySubscription.Charges.Should().BeEmpty();
    }

    [Fact]
    public async Task stripe_create_subscription_for_customer_fails()
    {
        // Arrange
        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;

        // Act
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, Subscriptions.BasicSubscription));

        // Assert
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeFalse();
        result.Payment.RequiresPaymentMethod().Should().BeTrue();
    }

    [Fact]
    public async Task stripe_create_subscription_for_customer_fails_with_sca()
    {
        // Act
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.SCA);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, new(paymentMethod.Id, IsDefault: true));
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, Subscriptions.BasicSubscription));

        // Assert
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeFalse();
        result.Payment.RequiresPaymentMethod().Should().BeFalse();
        result.Payment.RequiresAction().Should().BeTrue();
    }

    [Fact]
    public async Task stripe_create_subscription_with_multiple_items_for_customer()
    {
        // Arrange
        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = (await SystemUnderTest.CreateCustomerAsync(payCustomer)).ProcessorId;

        // Act
        PaySubscriptionResult result = await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new("Seats", Items: new()
        {
            new PaySubscribeOptionsItem(Subscriptions.BasicSubscription, 3),
            new PaySubscribeOptionsItem(Subscriptions.PremiumSubscription, 5),
        }));

        // Assert
        result.Should().NotBeNull();
        result.Payment.IsSucceeded().Should().BeFalse();
        result.Payment.RequiresPaymentMethod().Should().BeTrue();
        result.PaySubscription.SubscriptionItems.Should().NotBeNull();
        result.PaySubscription.SubscriptionItems.Should().HaveCount(2);
        result.PaySubscription.SubscriptionItems.First(s => s.Price.Id == Subscriptions.BasicSubscription).Quantity.Should().Be(3);
        result.PaySubscription.SubscriptionItems.First(s => s.Price.Id == Subscriptions.PremiumSubscription).Quantity.Should().Be(5);
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(StripeConfiguration.StripeClient, options);
    }
}