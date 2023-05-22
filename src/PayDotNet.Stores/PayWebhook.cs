namespace PayDotNet.Stores;

public class PayWebhook : Timestamps
{
    public string Event { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public Guid Id { get; set; }

    public string Processor { get; set; } = string.Empty;
}