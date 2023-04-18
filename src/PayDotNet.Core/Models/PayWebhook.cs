namespace PayDotNet.Core.Models;

public class PayWebhook
{
    public string Processor { get; set; }

    public string EventType { get; set; }

    // TODO: JSON
    public string Event { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

