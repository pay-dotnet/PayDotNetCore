using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Infrastructure;
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
            Id = "user_id",
            Email = "dotnetfromthemountain@gmail.com",
            Processor = "stripe",
            PaymentMethods = new[]
            {
                new PayPaymentMethod()
                {
                    CustomerId = "user_id",
                    ProcessorId = "pm_1Mxnbw2eZvKYlo2CMoBgn84m",
                    IsDefault = true,
                    Type = "card"
                }
            },
            Subscriptions = new[]
            {
                new PaySubscription()
                {
                    CustomerId = "user_id",
                    ProcessorId = "sub_123456",
                    ProcessorPlan = "default",
                    Status = PayStatus.Active,
                }
            }
        };



        // Mocks for Step 1
        Mocks<ICustomerManager>().Setup(m => m.CreateIfNotExistsAsync(newCustomer.Id, newCustomer.Email, newCustomer.Processor))
            .ReturnsAsync(newCustomer);

        // Mocks for Step 2
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateCustomerAsync(newCustomer.Email, new()))
            .ReturnsAsync(new PaymentProcessorCustomer("cus_123456", new()));
        Mocks<ICustomerManager>().Setup(m => m.UpdateAsync(It.IsAny<PayCustomer>()))
            .Returns(Task.CompletedTask);
        Mocks<IPaymentMethodManager>().Setup(m => m.CreateAsync(It.IsAny<PayCustomer>(), "payment_id", true))
            .ReturnsAsync(newCustomer.PaymentMethods.First());
        Mocks<ICustomerManager>().Setup(m => m.FindByIdAsync(newCustomer.Id, "stripe"))
            .ReturnsAsync(newCustomer);

        // Mocks for Step 3
        Mocks<IPaymentProcessorService>().Setup(s => s.CreateSubscriptionAsync(newCustomer, "default", new()))
            .ReturnsAsync(new PaymentProcessorSubscription("sub_123456", "cus_123456", new(), default));
        Mocks<ISubscriptionManager>().Setup(m => m.SynchroniseAsync("sub_123456", It.IsAny<object>(), "default", "", 0, 1))
            .ReturnsAsync(newCustomer.Subscriptions.First());

        // Act
        // Step 1:
        PayCustomer customer =
            await SystemUnderTest.SetPaymentProcessorAsync("user_id", "dotnetfromthemountain@gmail.com", PaymentProcessors.Stripe);

        // Step 2:
        customer = await SystemUnderTest.InitializeCustomerAsync(customer, "payment_id");
        customer.ProcessorId.Should().NotBeNullOrEmpty();
        customer.ProcessorId.Should().Be("cus_123456");
        customer.PaymentMethods.Should().HaveCount(1);

        // Step 3: subscribe
        PaySubscription subscription = await SystemUnderTest.SubscribeAsync(customer);
        subscription.Status.Should().Be(PayStatus.Active);
    }
}
