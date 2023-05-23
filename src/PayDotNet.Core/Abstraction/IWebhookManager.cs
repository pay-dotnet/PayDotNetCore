namespace PayDotNet.Core.Abstraction;

public interface IWebhookManager
{
    Task HandleAsync(PayWebhook payWebhook);
}