namespace PayDotNet.Core.Models;

public class PayWebhook : Timestamps
{
    // TODO: JSON
    public string Event { get; init; }

    public string EventType { get; init; }

    public string Processor { get; init; }
}