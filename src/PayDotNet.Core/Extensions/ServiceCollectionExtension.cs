using Microsoft.Extensions.Configuration;
using PayDotNet.Core;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Managers;
using PayDotNet.Core.Stores;
using PayDotNet.Core.Webhooks;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static PayDotNetBuilder AddPayDotNet(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddBillableManager<BillableManager>()
            .AddScoped<ICustomerManager, CustomerManager>()
            .AddScoped<IPaymentMethodManager, PaymentMethodManager>()
            .AddScoped<ISubscriptionManager, SubscriptionManager>()
            .AddScoped<IChargeManager, ChargeManager>()
            .AddScoped<IMerchantManager, MerchantManager>()
            .AddScoped<ICheckoutManager, CheckoutManager>()
            .AddSingleton<InMemoryStore>()
            .AddSingleton<IChargeStore>(sp => sp.GetRequiredService<InMemoryStore>())
            .AddSingleton<ICustomerStore>(sp => sp.GetRequiredService<InMemoryStore>())
            .AddSingleton<IPaymentMethodStore>(sp => sp.GetRequiredService<InMemoryStore>())
            .AddSingleton<ISubscriptionStore>(sp => sp.GetRequiredService<InMemoryStore>());

        // Webhook infrastructure registration
        services
            .AddScoped<IWebhookManager, WebhookManager>()
            .AddSingleton<IWebhookDispatcher, CompositeWebhookDispatcher>();

        // Configuration
        IConfigurationSection configSection = configuration.GetSection("PayDotNet");
        services.Configure<PayDotNetConfiguration>(configSection);
        return new(services, configuration, configSection.Get<PayDotNetConfiguration>());
    }

    public static IServiceCollection AddBillableManager<TBillableManager>(this IServiceCollection services)
        where TBillableManager : class, IBillableManager
    {
        return services.AddScoped<IBillableManager, TBillableManager>();
    }
}