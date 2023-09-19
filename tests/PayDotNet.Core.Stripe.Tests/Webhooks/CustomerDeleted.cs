using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Stripe.Webhooks;
using PayDotNet.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using Stripe;

namespace PayDotNet.Core.Stripe.Tests.Webhooks;

public class CustomerDeleted : StripeWebhookTest<CustomerDeletedHandler>
{
    [Fact]
    public async Task when_customer_deleted_should_handle_as_expected()
    {
        // Arrange
        var customerManager = ServiceProvider.GetRequiredService<ICustomerManager>();
        PayCustomer payCustomer = await customerManager.GetOrCreateCustomerAsync(PaymentProcessors.Stripe, "cus_NwOCARSB1GrrA6", "dotnetfromthemountain@gmail.com");
        payCustomer.DeletedAt.Should().BeNull();

        var billableManager = ServiceProvider.GetRequiredService<IBillableManager>();
        await billableManager.SubscribeAsync(payCustomer, new PaySubscribeOptions("default", "some-price-id"));

        // Act
        Event @event = GetStripeEvent(Events.CustomerDeleted);
        await SystemUnderTest.HandleAsync(@event);

        // Assert
        payCustomer = await billableManager.GetOrCreateCustomerAsync(PaymentProcessors.Stripe, "dotnetfromthemountain@gmail.com");
        payCustomer.DeletedAt.Should().NotBeNull();
        payCustomer.PaymentMethods.Should().BeEmpty();
        payCustomer.Subscriptions
    }
}

public class StripeWebhookTest<TSystemUnderTest> : TestBase<TSystemUnderTest>
{
    public readonly IServiceProvider ServiceProvider;

    public StripeWebhookTest()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .Build();

        ServiceCollection services = new();
        services.AddPayDotNet(configuration)
            .AddStripe()
            .AddEntityFrameworkStore<TestDbContext>();

        services.AddHttpClient<StripeClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler());

        services.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase("webhook_tests"));
        ServiceProvider = services.BuildServiceProvider();
    }

    protected override TSystemUnderTest CreateSystemUnderTest()
    {
        return ServiceProvider.GetRequiredService<TSystemUnderTest>();
    }

    protected Event GetStripeEvent(string eventName)
    {
        string json = System.IO.File.ReadAllText($"Webhooks\\Fixtures\\{eventName}.json");
        return new Event()
        {
            Data = EventData.FromJson(json),
        };
    }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyPayDotNetConfigurations();
        base.OnModelCreating(modelBuilder);
    }
}