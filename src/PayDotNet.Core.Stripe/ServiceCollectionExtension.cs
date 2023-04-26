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

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
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

        // Payment action is required to process an invoice
        builder.Webhooks.SubscribeWebhook<PaymentActionRequiredHandler>(Events.InvoicePaymentActionRequired);

        // If an invoice payment fails, we want to notify the user via email to update their payment details
        builder.Webhooks.SubscribeWebhook<PaymentFailedHandler>(Events.InvoicePaymentFailed);

        // If a subscription is manually created on Stripe, we want to sync
        builder.Webhooks.SubscribeWebhook<SubscriptionCreatedHandler>(Events.CustomerSubscriptionCreated);

        // If the plan, quantity, or trial ending date is updated on Stripe, we want to sync
        builder.Webhooks.SubscribeWebhook<SubscriptionUpdatedHandler>(Events.CustomerSubscriptionUpdated);

        // When a customers subscription is canceled, we want to update our records
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
        builder.Webhooks.SubscribeWebhook<AccountUpdatedHandler>(Events.PaymentIntentSucceeded);

        // Handle subscriptions in Stripe Checkout Sessions
        builder.Webhooks.SubscribeWebhook<CheckoutSessionAsyncPaymentSucceededHandler>(Events.PaymentIntentSucceeded);
        builder.Webhooks.SubscribeWebhook<CheckoutSessionCompletedHandler>(Events.PaymentIntentSucceeded);

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