using PayDotNet.Core.Webhooks;

namespace PayDotNet.Core;

public sealed class PaymentProcessorOptionsBuilder<TService>
{
    public WebhookRouterBuilder<TService> Webhooks { get; } = new();
}