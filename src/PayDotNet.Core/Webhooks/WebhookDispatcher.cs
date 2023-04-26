using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Webhooks;

public abstract class WebhookDispatcher : IWebhookDispatcher
{
    private readonly ILogger _logger;

    protected WebhookDispatcher(
        string name,
        WebhookRouterTable routingTable,
        IServiceProvider serviceProvider,
        ILogger logger)
    {
        Name = name;
        RoutingTable = routingTable;
        ServiceProvider = serviceProvider;
        _logger = logger;
    }

    public string Name { get; }

    public WebhookRouterTable RoutingTable { get; }

    public IServiceProvider ServiceProvider { get; }

    public abstract Task DispatchAsync(string processorName, string eventType, string @event);

    public IEnumerable<object> GetWebhookHandlers(string eventType)
    {
        if (!RoutingTable.ContainsKey(eventType))
        {
            // Unexpected event type
            _logger.LogWarning("Unhandled event type: {0}", eventType);
            yield break;
        }

        using var scope = ServiceProvider.CreateScope();
        foreach (Type serviceType in RoutingTable[eventType])
        {
            object? service = scope.ServiceProvider.GetService(serviceType);
            if (service is not null)
            {
                yield return service;
            }
        }
    }
}