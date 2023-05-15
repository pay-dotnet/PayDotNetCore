namespace PayDotNet.Core.Models;

public class PayWebhook : Timestamps
{
    public string Event { get; init; }

    public string EventType { get; init; }

    public Guid Id { get; set; }

    public string Processor { get; init; }
}