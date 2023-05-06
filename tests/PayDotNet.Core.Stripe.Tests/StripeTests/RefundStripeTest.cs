using Microsoft.Extensions.Options;
using PayDotNet.Core.Models;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public class RefundStripeTest : StripeTestBase<StripePaymentProcessorService>
{
    public RefundStripeTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task stripe_can_issue_credit_note_for_a_refund_for_Stripe_tax()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PaySubscriptionResult result =
            await SystemUnderTest.CreateSubscriptionAsync(payCustomer, new(string.Empty, "price_1MyDj2JUAL06t0UNphFwwc6l"));

        // Assert
        result.Payment.IsSucceeded().Should().BeTrue();

        // Act #2
        PayCharge payCharge = result.PaySubscription.Charges.Last();
        await SystemUnderTest.IssueCreditNotesAsync(payCharge, new(5_00, "Credit note"));

        // Assert #2
        result = await SystemUnderTest.GetSubscriptionAsync(result.PaySubscription.ProcessorId);
        result.PaySubscription.Charges.Last().Refunds.Should().NotBeEmpty();
        result.PaySubscription.Charges.Last().Refunds.Last().Amount.Should().Be(5_00);
    }

    [Fact]
    public async Task stripe_can_issue_refund()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PayChargeResult result = await SystemUnderTest.ChargeAsync(payCustomer, new(30_00, "eur"));
        result.PayCharge.Should().NotBeNull();

        // Act #2
        await SystemUnderTest.RefundAsync(result.PayCharge, new(5_00, "Refund"));

        // Assert #2
        PayCharge payCharge = await SystemUnderTest.GetChargeAsync(result.PayCharge.ProccesorId);
        payCharge.Should().NotBeNull();
        payCharge.Refunds.Should().NotBeEmpty();
        payCharge.Refunds.Last().Amount.Should().Be(5_00);
    }

    [Fact]
    public async Task stripe_can_issue_multiple_refunds()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PayChargeResult result = await SystemUnderTest.ChargeAsync(payCustomer, new(30_00, "eur"));
        result.PayCharge.Should().NotBeNull();

        // Act #2
        await SystemUnderTest.RefundAsync(result.PayCharge, new(15_00, "Refund"));
        await SystemUnderTest.RefundAsync(result.PayCharge, new(5_00, "Refund"));

        // Assert #2
        PayCharge payCharge = await SystemUnderTest.GetChargeAsync(result.PayCharge.ProccesorId);
        payCharge.Should().NotBeNull();
        payCharge.Refunds.Should().NotBeEmpty();
        payCharge.Refunds.First().Amount.Should().Be(15_00);
        payCharge.Refunds.Last().Amount.Should().Be(5_00);
    }

    [Fact]
    public async Task stripe_can_issue_refund_fails_because_more_than_initial_charge()
    {
        // Arrange
        PaymentMethod paymentMethod = await new PaymentMethodService().CreateAsync(PaymentMethods.Visa4242);

        PayCustomer payCustomer = NewCustomer;
        payCustomer.ProcessorId = await SystemUnderTest.CreateCustomerAsync(payCustomer);
        PayPaymentMethod payPaymentMethod = await SystemUnderTest.AttachPaymentMethodAsync(payCustomer, paymentMethod.Id, isDefault: true);
        payCustomer.PaymentMethods.Add(payPaymentMethod);

        // Act
        PayChargeResult result = await SystemUnderTest.ChargeAsync(payCustomer, new(30_00, "eur"));
        result.PayCharge.Should().NotBeNull();

        // Act #2
        Func<Task> action = () =>
            SystemUnderTest.RefundAsync(result.PayCharge, new(500_00, "Refund"));

        var exceptionAssertions = await action.Should().ThrowAsync<PayDotNetStripeException>();
        exceptionAssertions.Which.InnerException.Message.Should().Be("Refund amount (€500.00) is greater than charge amount (€30.00)");
    }

    protected override StripePaymentProcessorService CreateSystemUnderTest()
    {
        IOptions<PayDotNetConfiguration> options = Options.Create<PayDotNetConfiguration>(new());
        return new StripePaymentProcessorService(StripeConfiguration.StripeClient, options);
    }
}