using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Managers;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

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
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateCustomerAsync(newCustomer.Email, new()))
            .ReturnsAsync(new PaymentProcessorCustomer("cus_123456", new()));
        Mocks<ICustomerManager>().Setup(m => m.UpdateAsync(It.IsAny<PayCustomer>()))
            .Returns(Task.CompletedTask);
        Mocks<IPaymentMethodManager>().Setup(m => m.AddPaymentMethodAsync(It.IsAny<PaymentProcessorCustomer>(), "payment_id", true))
            .ReturnsAsync(newCustomer.PaymentMethods.First());
        Mocks<ICustomerManager>().Setup(m => m.FindByEmailAsync(newCustomer.Email, "stripe"))
            .ReturnsAsync(newCustomer);

        // Mocks for Step 2
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateSubscriptionAsync(newCustomer, "default", new()))
            .ReturnsAsync(new PaySubscriptionResult(newCustomer.Subscriptions.First(), null));

        // Act
        // Step 1: Initialize customer
        PayCustomer customer =
            await SystemUnderTest.GetOrCreateCustomerAsync("dotnetfromthemountain@gmail.com", PaymentProcessors.Stripe);
        customer.ProcessorId.Should().NotBeNullOrEmpty();
        customer.ProcessorId.Should().Be("cus_123456");
        customer.PaymentMethods.Should().HaveCount(1);

        // Step 2: subscribe
        PaySubscription subscription = await SystemUnderTest.SubscribeAsync(customer, "price_id", "subscription");
        subscription.Status.Should().Be(PaySubscriptionStatus.Active);
    }
}