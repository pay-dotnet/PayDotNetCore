using Microsoft.Extensions.DependencyInjection;

namespace PayDotNet.Core.Webhooks;

public sealed class WebhookRouterBuilder<TService>
{
    private readonly Dictionary<string, HashSet<Type>> RoutingTable = new();

    public WebhookRouterTable Build()
    {
        return new(RoutingTable);
    }

    public void Clear() => RoutingTable.Clear();

    public void Configure(IServiceCollection services)
    {
        foreach (var route in RoutingTable.Values)
        {
            foreach (var type in route)
            {
                services.AddScoped(type);
            }
        }
    }

    public void SubscribeWebhook<TWebhook>(string eventType)
                    where TWebhook : TService
    {
        if (!RoutingTable.ContainsKey(eventType))
        {
            RoutingTable.Add(eventType, new());
        }
        RoutingTable[eventType].Add(typeof(TWebhook));
    }

    public void UnsubscribeAllWebhooks(string eventType)
    {
        if (!RoutingTable.ContainsKey(eventType))
        {
            return;
        }

        RoutingTable[eventType].Clear();
    }

    public void UnsubscribeWebhook<TWebhook>(string eventType)
            where TWebhook : TService
    {
        if (!RoutingTable.ContainsKey(eventType))
        {
            return;
        }

        RoutingTable[eventType].Remove(typeof(TWebhook));
    }
}