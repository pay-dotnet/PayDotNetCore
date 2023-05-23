using Microsoft.EntityFrameworkCore;
using PayDotNet.Core;
using PayDotNet.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static PayDotNetBuilder AddEntityFrameworkStore<TContext>(this PayDotNetBuilder builder)
        where TContext : DbContext
    {
        builder.Services.AddScoped<ICustomerStore, PayCustomerStore<TContext>>();
        builder.Services.AddScoped<IChargeStore, PayChargeStore<TContext>>();
        builder.Services.AddScoped<IPaymentMethodStore, PayPaymentMethodStore<TContext>>();
        builder.Services.AddScoped<IMerchantStore, PayMerchantStore<TContext>>();
        builder.Services.AddScoped<ISubscriptionStore, PaySubscriptionStore<TContext>>();
        builder.Services.AddScoped<IWebhookStore, PayWebhookStore<TContext>>();

        return builder;
    }
}