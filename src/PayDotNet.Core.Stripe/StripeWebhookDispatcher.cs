using Microsoft.Extensions.Logging;
using PayDotNet.Core.Webhooks;
using Stripe;

namespace PayDotNet.Core.Stripe;

/// <summary>
/// Custom dispatcher for stripe so we can have strongly typed WebhookHandlers
/// </summary>
public sealed class StripeWebhookDispatcher : WebhookDispatcher
{
    public StripeWebhookDispatcher(
        WebhookRouterTable routingTable,
        IServiceProvider serviceProvider,
        ILogger logger)
        : base(PaymentProcessors.Stripe, routingTable, serviceProvider, logger)
    {
    }

    public override async Task DispatchAsync(string processorName, string eventType, string @event)
    {
        Event stripeEvent = EventUtility.ParseEvent(@event);
        foreach (object handler in GetWebhookHandlers(eventType))
        {
            if (handler is IStripeWebhookHandler stripeWebhookHandler)
            {
                await stripeWebhookHandler.HandleAsync(stripeEvent);
            }
        }
    }
}