using Microsoft.Extensions.Logging;
using PayDotNet.Core;
using PayDotNet.Core.Services;
using PayDotNet.Core.Stripe;
using PayDotNet.Core.Stripe.Webhooks;
using PayDotNet.Core.Webhooks;
using Stripe;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static PayDotNetBuilder AddStripe(this PayDotNetBuilder builder, Action<PaymentProcessorOptionsBuilder<IStripeWebhookHandler>>? configureOptionsAction = null)
    {
        // Make builder for payment processor.
        var options = new PaymentProcessorOptionsBuilder<IStripeWebhookHandler>()
            .ConfigureWebhookHandlers();

        // Allow integrators to customize their own webhooks or PaymentProcessor specific configuration.
        configureOptionsAction?.Invoke(options);

        builder
            .ConfigureStripeServices()
            .ConfigureStripeWebhookServices(options)
            .ConfigureWebhookController();

        return builder;
    }

    private static PaymentProcessorOptionsBuilder<IStripeWebhookHandler> ConfigureWebhookHandlers(this PaymentProcessorOptionsBuilder<IStripeWebhookHandler> builder)
    {
        builder.Webhooks.SubscribeWebhook<PaymentIntentSucceededHandler>(Events.PaymentIntentSucceeded);
        return builder;
    }

    private static PayDotNetBuilder ConfigureStripeServices(this PayDotNetBuilder builder)
    {
        // Stripe API configuration.
        StripeConfiguration.ApiKey = builder.PayDotNetConfiguration.Stripe.ApiKey;
        StripeConfiguration.AppInfo = new()
        {
            Name = "PayDotNetCore",
            PartnerId = "TODO",
            Url = "https://github.com/pay-dotnet/PayDotNetCore"
        };
        builder.Services.AddSingleton<IPaymentProcessorService, StripePaymentProcessorService>();
        return builder;
    }

    private static PayDotNetBuilder ConfigureStripeWebhookServices(this PayDotNetBuilder builder, PaymentProcessorOptionsBuilder<IStripeWebhookHandler> options)
    {
        WebhookRouterTable routingTable = options.Webhooks.Build();
        options.Webhooks.Configure(builder.Services);
        builder.Services.AddSingleton<WebhookDispatcher>(serviceProvider =>
        {
            return new StripeWebhookDispatcher(routingTable, serviceProvider, serviceProvider.GetRequiredService<ILogger<StripeWebhookDispatcher>>());
        });

        return builder;
    }

    private static PayDotNetBuilder ConfigureWebhookController(this PayDotNetBuilder builder)
    {
        builder.Services
            .AddMvcCore()
            .AddApplicationPart(typeof(StripePaymentProcessorService).Assembly);
        return builder;
    }
}