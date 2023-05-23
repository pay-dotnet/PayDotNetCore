namespace PayDotNet.Stores;

public interface IWebhookStore : IModelStore<PayWebhook>
{
    IQueryable<PayWebhook> Webhooks { get; }
}