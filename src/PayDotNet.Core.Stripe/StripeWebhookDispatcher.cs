using Microsoft.Extensions.Logging;
using PayDotNet.Core.Webhooks;
using Stripe;

namespace PayDotNet.Core.Stripe;

/// <summary>
/// Custom dispatcher for stripe so we can have strongly typed WebhookHandlers
/// </summary>
public sealed class StripeWebhookDispatcher : WebhookDispatcher
{
    public StripeWebhookDispatcher(
        WebhookRouterTable routingTable,
        IServiceProvider serviceProvider,
        ILogger logger)
        : base(PaymentProcessors.Stripe, routingTable, serviceProvider, logger)
    {
    }

    public override async Task DispatchAsync(PayWebhook payWebhook)
    {
        Event stripeEvent = EventUtility.ParseEvent(payWebhook.Event);
        foreach (object handler in GetWebhookHandlers(payWebhook.EventType))
        {
            if (handler is IStripeWebhookHandler stripeWebhookHandler)
            {
                try
                {
                    await stripeWebhookHandler.HandleAsync(stripeEvent);
                }
                catch (PayDotNetException payDotNetException)
                {
                    Logger.LogError(payDotNetException, string.Format("Handler '{0}' caused a known exception", handler.GetType()));
                }
            }
            else
            {
                Logger.LogWarning(string.Format("Handler of type '{0}' does not implement contract interface {1}", handler.GetType(), typeof(IStripeWebhookHandler)));
            }
        }
    }
}


public class Solution {
    public bool CanJump(int[] nums) {
        bool[] visited = new bool[nums.Length];
        for(int i = nums[0]; i > 0; i--)
        {
            int cursor = i;
            Console.Write($"START ");
            Console.Write($"--> {cursor} ");
            visited[cursor] = true;
            while(true)
            {   
                if(cursor == nums.Length -1)
                {
                    Console.WriteLine($"--> END");
                    return true;
                }

                cursor += nums[cursor];
                visited[cursor] = true;
                Console.Write($"--> {cursor} ");
                if(cursor > nums.Length - 1)
                {
                    break;
                }

            }
            Console.WriteLine($"Can't jump {i}");
        }

        return false;
        
    }
}