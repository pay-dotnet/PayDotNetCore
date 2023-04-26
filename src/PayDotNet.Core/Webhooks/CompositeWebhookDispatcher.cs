using Microsoft.Extensions.Logging;
using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Webhooks;

/// <summary>
/// Responsible for routing the specific ProcessorName to the correct WebhookDispatcher
/// </summary>
public class CompositeWebhookDispatcher : IWebhookDispatcher
{
    private readonly Dictionary<string, WebhookDispatcher> _dispatchers;
    private readonly ILogger<CompositeWebhookDispatcher> _logger;

    public CompositeWebhookDispatcher(
        IEnumerable<WebhookDispatcher> dispatchers,
        ILogger<CompositeWebhookDispatcher> logger)
    {
        _dispatchers = dispatchers.ToDictionary(d => d.Name);
        _logger = logger;
    }

    public Task DispatchAsync(string processorName, string eventType, string @event)
    {
        if (!_dispatchers.ContainsKey(processorName))
        {
            _logger.LogWarning("Unhandled processor name: {0}", processorName);
            return Task.CompletedTask;
        }

        WebhookDispatcher dispatcher = _dispatchers[processorName];
        return dispatcher.DispatchAsync(processorName, eventType, @event);
    }
}