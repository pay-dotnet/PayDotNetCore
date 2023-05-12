namespace PayDotNet.Core.Models;

public class PayWebhook : Timestamps
{
    public string Processor { get; init; }

    public string EventType { get; init; }

    // TODO: JSON
    public string Event { get; init; }
}