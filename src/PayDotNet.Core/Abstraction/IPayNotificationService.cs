using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPayNotificationService
{
    Task OnChargeRefundedAsync(PayCustomer payCustomer, PayCharge payCharge);

    Task OnChargeSucceededAsync(PayCustomer payCustomer, PayCharge payCharge);

    Task OnPaymentActionRequiredAsync(PayCustomer payCustomer, PaySubscription paySubscription, IPayment payment);

    Task OnPaymentFailedAsync(PayCustomer payCustomer, PaySubscription paySubscription, IPayment payment);

    Task OnSubscriptionRenewingAsync(PayCustomer payCustomer, PaySubscription paySubscription);

    Task OnSubscriptionTrialEndedAsync(PayCustomer payCustomer, PaySubscription paySubscription);

    Task OnSubscriptionTrialEndingAsync(PayCustomer payCustomer, PaySubscription paySubscription);
}