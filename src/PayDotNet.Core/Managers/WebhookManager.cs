using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Managers;

public class WebhookManager : IWebhookManager
{
    private readonly IWebhookDispatcher _webhookDispatcher;
    private readonly IWebhookStore _webhookStore;

    public WebhookManager(
        IWebhookStore webhookStore,
        IWebhookDispatcher webhookDispatcher)
    {
        _webhookStore = webhookStore;
        _webhookDispatcher = webhookDispatcher;
    }

    public virtual async Task HandleAsync(PayWebhook payWebhook)
    {
        PayWebhook? existingPayWebhook = _webhookStore.Webhooks.FirstOrDefault(w => w.Id == payWebhook.Id);

        // Ignore creation if already exists. Must be a retry.
        if (existingPayWebhook is null)
        {
            await _webhookStore.CreateAsync(payWebhook);
        }

        await _webhookDispatcher.DispatchAsync(payWebhook);
        await _webhookStore.DeleteAsync(payWebhook);
    }
}