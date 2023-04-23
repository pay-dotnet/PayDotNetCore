using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Managers;
using PayDotNet.Core.Stores;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static PayDotNetBuilder AddPayDotNet(this IServiceCollection services)
    {
        services
            .AddBillableManager<BillableManager>()
            .AddScoped<ICustomerManager, CustomerManager>()
            .AddScoped<IPaymentMethodManager, PaymentMethodManager>()
            .AddScoped<ISubscriptionManager, SubscriptionManager>()
            .AddScoped<IChargeManager, ChargeManager>()
            .AddSingleton<InMemoryStore>()
            .AddSingleton<ICustomerStore>(sp => sp.GetRequiredService<InMemoryStore>())
            .AddSingleton<IPaymentMethodStore>(sp => sp.GetRequiredService<InMemoryStore>())
            .AddSingleton<ISubscriptionStore>(sp => sp.GetRequiredService<InMemoryStore>())
            ;
        return new(services);
    }

    public static IServiceCollection AddBillableManager<TBillableManager>(this IServiceCollection services)
        where TBillableManager : class, IBillableManager
    {
        return services.AddScoped<IBillableManager, TBillableManager>();
    }
}

public class PayDotNetBuilder
{
    public readonly IServiceCollection Services;

    public PayDotNetBuilder(IServiceCollection services)
    {
        Services = services;
    }
}