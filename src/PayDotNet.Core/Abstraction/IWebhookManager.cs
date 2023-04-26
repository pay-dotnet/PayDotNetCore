namespace PayDotNet.Core.Abstraction;

public interface IWebhookManager
{
    Task CreateAsync(string processor, string eventType, string @event);
}