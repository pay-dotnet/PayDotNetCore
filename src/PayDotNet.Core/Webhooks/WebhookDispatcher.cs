using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Webhooks;

public class WebhookDispatcher : IWebhookDispatcher
{
    private readonly Dictionary<string, PaymentProcessorWebhookDispatcher> _dispatchers;

    public WebhookDispatcher(IEnumerable<PaymentProcessorWebhookDispatcher> dispatchers)
    {
        _dispatchers = dispatchers.ToDictionary(d => d.Name);
    }

    public Task DispatchAsync(string processorName, string eventType, string @event)
    {
        if (!_dispatchers.ContainsKey(processorName))
        {
            return Task.CompletedTask;
        }

        PaymentProcessorWebhookDispatcher dispatcher = _dispatchers[processorName];
        return dispatcher.DispatchAsync(eventType, @event);
    }
}