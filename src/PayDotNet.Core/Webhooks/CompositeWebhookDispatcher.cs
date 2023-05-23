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

    public Task DispatchAsync(PayWebhook payWebhook)
    {
        if (!_dispatchers.ContainsKey(payWebhook.Processor))
        {
            _logger.LogWarning("Unhandled processor name: {0}", payWebhook.Processor);
            return Task.CompletedTask;
        }

        WebhookDispatcher dispatcher = _dispatchers[payWebhook.Processor];
        return dispatcher.DispatchAsync(payWebhook);
    }
}