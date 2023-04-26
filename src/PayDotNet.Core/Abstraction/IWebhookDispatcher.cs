namespace PayDotNet.Core.Abstraction;

public interface IWebhookDispatcher
{
    Task DispatchAsync(string processorName, string eventType, string @event);
}