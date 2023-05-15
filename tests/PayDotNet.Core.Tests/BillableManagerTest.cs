using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Managers;

namespace PayDotNet.Core.Tests;

public class BillableManagerTest : TestBase<BillableManager>
{
    [Fact]
    public async Task when_subscribing_new_customer_should_have_active_subscription()
    {
        // Arrange
        PayCustomer newCustomer = new()
        {
            Email = "dotnetfromthemountain@gmail.com",
            Processor = "stripe",
            PaymentMethods = new[]
            {
                new PayPaymentMethod()
                {
                    ProcessorId = "pm_1Mxnbw2eZvKYlo2CMoBgn84m",
                    IsDefault = true,
                    Type = "card"
                }
            },
            Subscriptions = new[]
            {
                new PaySubscription()
                {
                    ProcessorId = "sub_123456",
                    ProcessorPlan = "default",
                    Status = PaySubscriptionStatus.Active,
                }
            }
        };

        // Mocks for Step 1
        Mocks<ICustomerManager>().Setup(m => m.GetOrCreateCustomerAsync(newCustomer.Email, newCustomer.Processor))
            .ReturnsAsync(newCustomer);
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateCustomerAsync(newCustomer))
            .ReturnsAsync("cus_123456");
        Mocks<ICustomerManager>().Setup(m => m.SoftDeleteAsync(It.IsAny<PayCustomer>()))
            .Returns(Task.CompletedTask);
        Mocks<IPaymentMethodManager>().Setup(m => m.AddPaymentMethodAsync(It.IsAny<PayCustomer>(), new PayPaymentMethodOptions("payment_id", true)))
            .ReturnsAsync(newCustomer.PaymentMethods.First());
        Mocks<ICustomerManager>().Setup(m => m.FindByEmailAsync(PaymentProcessors.Stripe, newCustomer.Email))
            .ReturnsAsync(newCustomer);

        // Mocks for Step 2
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateSubscriptionAsync(newCustomer, It.IsAny<PaySubscribeOptions>()))
            .ReturnsAsync(new PaySubscriptionResult(newCustomer.Subscriptions.First(), null));

        // Act
        // Step 1: Initialize customer
        PayCustomer customer =
            await SystemUnderTest.GetOrCreateCustomerAsync("dotnetfromthemountain@gmail.com", new(PaymentProcessors.Stripe));
        customer.ProcessorId.Should().NotBeNullOrEmpty();
        customer.ProcessorId.Should().Be("cus_123456");
        customer.PaymentMethods.Should().HaveCount(1);

        // Step 2: subscribe
        IPayment payment = await SystemUnderTest.SubscribeAsync(customer, new("subscription_name", "price_id"));
        payment.Status.Should().Be(PayStatus.Succeeded);
    }
}

public class BillableManagerCheckoutTest : TestBase<BillableManager>
{
    // https://stripe.com/docs/api/checkout/sessions/create
    //
    // checkout_charge(amount: 15_00, name: "T-shirt", quantity: 2)
    //
    // checkout(mode: "payment")
    // checkout(mode: "setup")
    // checkout(mode: "payment")
    //
    // checkout(line_items: "price_12345", quantity: 2)
    // checkout(line_items: [{ price: "price_123" }, { price: "price_456" }])
    // checkout(line_items: "price_12345", allow_promotion_codes: true)
}