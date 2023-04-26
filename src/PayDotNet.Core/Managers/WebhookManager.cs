using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Managers;

public class WebhookManager : IWebhookManager
{
    private readonly IWebhookDispatcher _webhookDispatcher;

    public WebhookManager(IWebhookDispatcher webhookDispatcher)
    {
        _webhookDispatcher = webhookDispatcher;
    }

    public Task CreateAsync(string processor, string eventType, string @event)
    {
        return _webhookDispatcher.DispatchAsync(processor, eventType, @event);
        //return _webhooksStore.CreateAsync(new()
        //{
        //    Processor = processor,
        //    EventType = eventType,

        //    // TODO: json
        //    Event = @event.ToString(),

        //    CreatedAt = DateTime.UtcNow,
        //    UpdatedAt = DateTime.UtcNow,
        //});
    }
}