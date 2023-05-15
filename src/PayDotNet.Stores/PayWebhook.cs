namespace PayDotNet.Stores;

public class PayWebhook : Timestamps
{
    public string Event { get; set; }

    public string EventType { get; set; }

    public Guid Id { get; set; }

    public string Processor { get; set; }
}