namespace PayDotNet.Core.Models;

public class PayWebhook
{
    public string Processor { get; init; }

    public string EventType { get; init; }

    // TODO: JSON
    public string Event { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}