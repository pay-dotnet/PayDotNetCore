using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Services;

internal sealed class PayNullNotificationService : IPayNotificationService
{
    public Task OnChargeRefundedAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        return Task.CompletedTask;
    }

    public Task OnChargeSucceededAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
    }

    public Task OnPaymentActionRequiredAsync(PayCustomer payCustomer, PaySubscription paySubscription, IPayment payment)
    {
        return Task.CompletedTask;
    }

    public Task OnPaymentFailedAsync(PayCustomer payCustomer, PaySubscription paySubscription, IPayment payment)
    {
        return Task.CompletedTask;
    }

    public Task OnSubscriptionTrialEndedAsync(PayCustomer payCustomer, PaySubscription paySubscription)
    {
        return Task.CompletedTask;
    }

    public Task OnSubscriptionRenewingAsync(PayCustomer payCustomer, PaySubscription paySubscription)
    {
        return Task.CompletedTask;
    }

    public Task OnSubscriptionTrialEndingAsync(PayCustomer payCustomer, PaySubscription paySubscription)
    {
        return Task.CompletedTask;
    }
}