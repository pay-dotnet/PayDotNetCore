using System.Collections.ObjectModel;

namespace PayDotNet.Core.Webhooks;

/// <summary>
/// ReadOnly routing table for the webhooks
/// </summary>
public sealed class WebhookRouterTable : ReadOnlyDictionary<string, HashSet<Type>>
{
    public WebhookRouterTable(IDictionary<string, HashSet<Type>> dictionary) : base(dictionary)
    {
    }
}