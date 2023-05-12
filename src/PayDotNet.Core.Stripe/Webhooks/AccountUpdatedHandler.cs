using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Webhooks;

public class AccountUpdatedHandler : IStripeWebhookHandler
{
    private readonly IMerchantManager _merchantManager;

    public AccountUpdatedHandler(IMerchantManager merchantManager)
    {
        _merchantManager = merchantManager;
    }

    public async Task HandleAsync(Event @event)
    {
        if (@event.Data.Object is Account account)
        {
            PayMerchant? payMerchant = await _merchantManager.FindByIdAsync(PaymentProcessors.Stripe, account.Id);
            if (payMerchant is not null)
            {
                payMerchant.IsOnboardingComplete = account.ChargesEnabled;
                await _merchantManager.UpdateAsync(payMerchant);
            }
        }
    }
}