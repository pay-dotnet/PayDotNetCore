using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Managers;

public class WebhookManager : IWebhookManager
{
    private readonly IWebhookDispatcher _webhookDispatcher;

    public WebhookManager(IWebhookDispatcher webhookDispatcher)
    {
        _webhookDispatcher = webhookDispatcher;
    }

    public virtual Task CreateAsync(string processor, string eventType, string @event)
    {
        // TODO: refactor to PayWebhook class.
        return _webhookDispatcher.DispatchAsync(processor, eventType, @event);
    }
}