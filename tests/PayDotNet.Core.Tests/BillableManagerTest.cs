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
                    Status = PayStatus.Active,
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
        Mocks<IPaymentMethodManager>().Setup(m => m.CreateAsync(It.IsAny<PayCustomer>(), "payment_id", true))
            .ReturnsAsync(newCustomer.PaymentMethods.First());
        Mocks<ICustomerManager>().Setup(m => m.FindByEmailAsync(newCustomer.Email, "stripe"))
            .ReturnsAsync(newCustomer);

        // Mocks for Step 2
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateSubscriptionAsync(newCustomer, "default", new()))
            .ReturnsAsync(new PaymentProcessorSubscription("sub_123456", "cus_123456", new(), default));
        Mocks<ISubscriptionManager>().Setup(m => m.SynchroniseAsync("sub_123456", It.IsAny<object>(), "default", "", 0, 1))
            .ReturnsAsync(newCustomer.Subscriptions.First());

        // Act
        // Step 1: Initialize customer
        PayCustomer customer =
            await SystemUnderTest.SetPaymentProcessorAsync("dotnetfromthemountain@gmail.com", PaymentProcessors.Stripe, "payment_id");
        customer.ProcessorId.Should().NotBeNullOrEmpty();
        customer.ProcessorId.Should().Be("cus_123456");
        customer.PaymentMethods.Should().HaveCount(1);

        // Step 2: subscribe
        PaySubscription subscription = await SystemUnderTest.SubscribeAsync(customer);
        subscription.Status.Should().Be(PayStatus.Active);
    }
}