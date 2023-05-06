using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly IChargeManager _chargeManager;
    private readonly ISubscriptionStore _subscriptionStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public SubscriptionManager(
        IChargeManager chargeManager,
        ISubscriptionStore subscriptionStore,
        CompositePaymentProcessorService paymentProcessorService)
    {
        _chargeManager = chargeManager;
        _subscriptionStore = subscriptionStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public virtual async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        // TODO: BrainTree vs Stripe logic is different
        PaySubscriptionResult result = await _paymentProcessorService.CreateSubscriptionAsync(payCustomer, options);

        await SynchroniseAsync(result.PaySubscription.ProcessorId, result, payCustomer);

        return result;
    }

    public virtual Task<PaySubscription?> FindByIdAsync(string processorId, string customerId)
    {
        return Task.FromResult(_subscriptionStore.Subscriptions.FirstOrDefault(s => s.ProcessorId == processorId && s.CustomerId == customerId));
    }

    public virtual async Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options)
    {
        paySubscription.EndsAt = paySubscription.IsOnTrial()
            ? paySubscription.TrailEndsAt!.Value
            : paySubscription.CurrentPeriodEnd!.Value;

        // First cancel the payment processor, then save the outcome.
        await _paymentProcessorService.CancelAsync(payCustomer, paySubscription, options);
        await _subscriptionStore.UpdateAsync(paySubscription);
    }

    public virtual async Task CancelNowAsync(PayCustomer payCustomer, PaySubscription paySubscription)
    {
        paySubscription.CancelNow();

        // First cancel the payment processor, then save the outcome.
        await _paymentProcessorService.CancelAsync(payCustomer, paySubscription, new(CancelAtEndPeriod: false));
        await _subscriptionStore.UpdateAsync(paySubscription);
    }

    /// <remarks>
    /// We manually cancel and save all subscriptions since the customer was deleted.
    /// </remarks>
    public virtual Task CancellAllAsync(PayCustomer payCustomer)
    {
        ICollection<PaySubscription> customerSubscriptions = _subscriptionStore.Subscriptions.Where(sub => sub.CustomerId == payCustomer.Id).ToList();
        foreach (PaySubscription paySubscription in customerSubscriptions)
        {
            paySubscription.CancelNow();
        }

        return _subscriptionStore.UpdateAsync(customerSubscriptions);
    }

    public async Task SynchroniseAsync(string processorId, PaySubscriptionResult? @object, PayCustomer payCustomer)
    {
        @object ??= await _paymentProcessorService.GetSubscriptionAsync(payCustomer, processorId);
        if (@object == null)
        {
            return;
        }

        // Fix link to Pay Customer.
        @object.PaySubscription.CustomerId = payCustomer.Id;

        PaySubscription? existingPaySubscription = _subscriptionStore.Subscriptions.FirstOrDefault(s => s.ProcessorId == @object.PaySubscription.ProcessorId);
        if (existingPaySubscription is null)
        {
            await _subscriptionStore.CreateAsync(@object.PaySubscription);
        }
        else
        {
            // If pause behavior is changing to `void`, record the pause start date
            // Any other pause status (or no pause at all) should have nil for start
            if (existingPaySubscription.PauseBehaviour != @object.PaySubscription.PauseBehaviour && @object.PaySubscription.PauseBehaviour == "void")
            {
                existingPaySubscription.PauseStartsAt = existingPaySubscription.CurrentPeriodEnd;
            }
            await _subscriptionStore.UpdateAsync(@object.PaySubscription);
        }

        PayCharge? charge = @object.PaySubscription.Charges.LastOrDefault();
        if (charge is not null && charge.Status == PayStatus.Succeeded)
        {
            await _chargeManager.SynchroniseAsync(payCustomer.Processor, charge.ProccesorId, payCustomer.ProcessorId);
        }
    }
}