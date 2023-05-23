namespace PayDotNet.Core.Abstraction;

public interface IWebhookDispatcher
{
    Task DispatchAsync(PayWebhook payWebhook);
}