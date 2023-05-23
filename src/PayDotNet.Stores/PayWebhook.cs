namespace PayDotNet.Stores;

public class PayWebhook : Timestamps
{
    public PayWebhook()
    {
    }

    public PayWebhook(string id, string processor, string eventType, string @event, DateTimeOffset createdAt)
    {
        Id = id;
        Processor = processor;
        EventType = eventType;
        Event = @event;
        CreatedAt = createdAt;
    }

    public string Event { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public string Processor { get; set; } = string.Empty;
}