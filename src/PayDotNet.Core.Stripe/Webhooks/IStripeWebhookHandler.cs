using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

/// <summary>
/// Strongly typed interface for handling the Stripe webhooks.
/// </summary>
public interface IStripeWebhookHandler
{
    Task HandleAsync(Event @event);
}