using Stripe;

namespace PayDotNet.Core.Stripe;

/// <summary>
/// Strongly typed interface for handling the Stripe webhooks.
/// </summary>
public interface IStripeWebhookHandler
{
    Task HandleAsync(Event @event);
}