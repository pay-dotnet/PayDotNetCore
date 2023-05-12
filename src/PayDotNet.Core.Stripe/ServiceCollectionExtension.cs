using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayDotNet.Core;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Stripe;
using PayDotNet.Core.Stripe.Webhooks;
using PayDotNet.Core.Webhooks;
using Stripe;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds the PayDotNetCore.Stripe related services and configuration.
    /// </summary>
    /// <param name="builder">The builder used in this package.</param>
    /// <param name="configureOptionsAction">Action where you can configure additional webhooks or remove the existing ones that come out of the box.</param>
    /// <returns></returns>
    public static PayDotNetBuilder AddStripe(this PayDotNetBuilder builder, Action<PaymentProcessorOptionsBuilder<IStripeWebhookHandler>>? configureOptionsAction = null)
    {
        // Make builder for payment processor.
        var options = new PaymentProcessorOptionsBuilder<IStripeWebhookHandler>()
            .ConfigureWebhookHandlers();

        // Allow integrators to customize their own webhooks or PaymentProcessor specific configuration.
        configureOptionsAction?.Invoke(options);

        builder.Services
            .ConfigureStripeClient()
            .ConfigureStripeServices()
            .ConfigureStripeWebhookServices(options)
            .ConfigureWebhookController();

        return builder;
    }

    /// <remarks>
    /// For reference, see: https://github.com/pay-rails/pay/blob/master/lib/pay/stripe.rb#L39
    /// </remarks>
    private static PaymentProcessorOptionsBuilder<IStripeWebhookHandler> ConfigureWebhookHandlers(this PaymentProcessorOptionsBuilder<IStripeWebhookHandler> builder)
    {
        // Listen to the charge event to make sure we get non-subscription
        // purchases as well. Invoice is only for subscriptions and manual creation
        // so it does not include individual charges.
        builder.Webhooks.SubscribeWebhook<ChargeSucceededHandler>(Events.ChargeSucceeded);
        builder.Webhooks.SubscribeWebhook<ChargeRefundedHandler>(Events.ChargeRefunded);

        builder.Webhooks.SubscribeWebhook<PaymentIntentSucceededHandler>(Events.PaymentIntentSucceeded);

        // Warn user of upcoming charges for their subscription. This is handy for
        // notifying annual users their subscription will renew shortly.
        // This probably should be ignored for monthly subscriptions.
        builder.Webhooks.SubscribeWebhook<SubscriptionRenewingHandler>(Events.InvoiceUpcoming);

        // IPayment action is required to process an invoice
        builder.Webhooks.SubscribeWebhook<PaymentActionRequiredHandler>(Events.InvoicePaymentActionRequired);

        // If an invoice payment fails, we want to notify the user via email to update their payment details
        builder.Webhooks.SubscribeWebhook<PaymentFailedHandler>(Events.InvoicePaymentFailed);

        // If a subscription is manually created on Stripe, we want to sync
        builder.Webhooks.SubscribeWebhook<SubscriptionCreatedHandler>(Events.CustomerSubscriptionCreated);

        // If the plan, quantity, or trial ending date is updated on Stripe, we want to sync
        builder.Webhooks.SubscribeWebhook<SubscriptionUpdatedHandler>(Events.CustomerSubscriptionUpdated);

        // When a customers subscription is Canceled, we want to update our records
        builder.Webhooks.SubscribeWebhook<SubscriptionDeletedHandler>(Events.CustomerSubscriptionDeleted);

        // When a customers subscription trial period is 3 days from ending or ended immediately this event is fired
        builder.Webhooks.SubscribeWebhook<SubscriptionTrailWillEndHandler>(Events.CustomerSubscriptionTrialWillEnd);

        // Monitor changes for customer's default card changing
        builder.Webhooks.SubscribeWebhook<CustomerUpdatedHandler>(Events.CustomerUpdated);

        // If a customer was deleted in Stripe, their subscriptions should be cancelled
        builder.Webhooks.SubscribeWebhook<CustomerDeletedHandler>(Events.CustomerDeleted);

        // If a customer's payment source was deleted in Stripe, we should update as well
        builder.Webhooks.SubscribeWebhook<PaymentMethodAttachedHandler>(Events.PaymentMethodAttached);
        builder.Webhooks.SubscribeWebhook<PaymentMethodUpdatedHandler>(Events.PaymentMethodUpdated);
        builder.Webhooks.SubscribeWebhook<PaymentMethodUpdatedHandler>(Events.PaymentMethodAutomaticallyUpdated);
        builder.Webhooks.SubscribeWebhook<PaymentMethodDetachedHandler>(Events.PaymentMethodDetached);

        // If an account is updated in stripe, we should update it as well
        builder.Webhooks.SubscribeWebhook<AccountUpdatedHandler>(Events.AccountUpdated);

        // Handle subscriptions in Stripe Checkout Sessions
        builder.Webhooks.SubscribeWebhook<CheckoutSessionAsyncPaymentSucceededHandler>(Events.CheckoutSessionAsyncPaymentSucceeded);
        builder.Webhooks.SubscribeWebhook<CheckoutSessionCompletedHandler>(Events.CheckoutSessionCompleted);

        return builder;
    }

    public static IServiceCollection ConfigureStripeClient(this IServiceCollection services)
    {
        // Add HttpClient.
        services.AddHttpClient<StripeClient>();

        // Stripe API configuration.
        services.AddSingleton<IStripeClient, StripeClient>(CreateStripeClient);
        return services;
    }

    private static StripeClient CreateStripeClient(IServiceProvider serviceProvider)
    {
        // HttpClient
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient httpClient = httpClientFactory.CreateClient(nameof(StripeClient));
        SystemNetHttpClient stripeHttpClient = new(httpClient, appInfo: StripePaymentProcessorService.AppInfo);

        // Options
        IOptions<PayDotNetConfiguration> options = serviceProvider.GetRequiredService<IOptions<PayDotNetConfiguration>>();

        return new StripeClient(apiKey: options.Value.Stripe.ApiKey, httpClient: stripeHttpClient);
    }

    public static IServiceCollection ConfigureStripeServices(this IServiceCollection services)
    {
        services.AddSingleton<StripePaymentProcessorService>();
        services.AddSingleton<IPaymentProcessorService>(sp => sp.GetRequiredService<StripePaymentProcessorService>());
        return services;
    }

    private static IServiceCollection ConfigureStripeWebhookServices(this IServiceCollection services, PaymentProcessorOptionsBuilder<IStripeWebhookHandler> options)
    {
        WebhookRouterTable routingTable = options.Webhooks.Build();
        options.Webhooks.Configure(services);
        services.AddSingleton<WebhookDispatcher>(serviceProvider =>
        {
            return new StripeWebhookDispatcher(routingTable, serviceProvider, serviceProvider.GetRequiredService<ILogger<StripeWebhookDispatcher>>());
        });

        return services;
    }

    private static IServiceCollection ConfigureWebhookController(this IServiceCollection services)
    {
        services
            .AddMvcCore()
            .AddApplicationPart(typeof(StripePaymentProcessorService).Assembly);
        return services;
    }
}