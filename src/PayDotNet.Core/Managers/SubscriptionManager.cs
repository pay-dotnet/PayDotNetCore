using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly IChargeManager _chargeManager;
    private readonly IPaymentProcessorService _paymentProcessorService;
    private readonly ISubscriptionStore _subscriptionStore;

    public SubscriptionManager(
        IChargeManager chargeManager,
        ISubscriptionStore subscriptionStore,
        CompositePaymentProcessorService paymentProcessorService)
    {
        _chargeManager = chargeManager;
        _subscriptionStore = subscriptionStore;
        _paymentProcessorService = paymentProcessorService;
    }

    /// <inheritdoc/>
    public virtual async Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options)
    {
        paySubscription.EndsAt = paySubscription.IsOnTrial()
            ? paySubscription.TrailEndsAt!.Value
            : paySubscription.CurrentPeriodEnd!.Value;

        // First cancel the subscription in the payment processor to make sure no payments are going through anymore.
        // Then safe the result. This makes sure
        await _paymentProcessorService.CancelAsync(payCustomer, paySubscription, options);
        await _subscriptionStore.UpdateAsync(paySubscription);
    }

    /// <inheritdoc/>
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

        return _subscriptionStore.UpdateAllAsync(customerSubscriptions);
    }

    /// <inheritdoc/>
    public virtual async Task CancelNowAsync(PayCustomer payCustomer, PaySubscription paySubscription)
    {
        paySubscription.CancelNow();

        // First cancel the payment processor, then save the outcome.
        await _paymentProcessorService.CancelAsync(payCustomer, paySubscription, new(CancelAtEndPeriod: false));
        await _subscriptionStore.UpdateAsync(paySubscription);
    }

    /// <inheritdoc/>
    public virtual async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        PaySubscriptionResult result = await _paymentProcessorService.CreateSubscriptionAsync(payCustomer, options);
        await SynchroniseAsync(payCustomer, result);
        return result;
    }

    /// <inheritdoc/>
    public virtual Task<PaySubscription?> FindByIdAsync(PayCustomer payCustomer, string processorId)
    {
        return Task.FromResult(_subscriptionStore.Subscriptions.FirstOrDefault(s => s.CustomerId == payCustomer.Id && s.ProcessorId == processorId));
    }

    /// <inheritdoc/>
    public async Task<PaySubscription?> SynchroniseAsync(PayCustomer payCustomer, string processorId)
    {
        PaySubscriptionResult? result = await _paymentProcessorService.GetSubscriptionAsync(payCustomer, processorId);
        if (result is null)
        {
            return null;
        }
        return await SynchroniseAsync(payCustomer, result);
    }

    /// <inheritdoc/>
    public Task<PaySubscription?> SynchroniseAsync(PayCustomer payCustomer, PaySubscriptionResult paySubscriptionResult)
    {
        return Synchronizer.Retry(() => SynchroniseAsyncInternal(payCustomer, paySubscriptionResult));
    }

    private async Task<PaySubscription?> SynchroniseAsyncInternal(PayCustomer payCustomer, PaySubscriptionResult paySubscriptionResult)
    {
        // Fix link to Pay Customer
        paySubscriptionResult.PaySubscription.CustomerId = payCustomer.Id;

        PaySubscription? paySubscription = _subscriptionStore.Subscriptions.FirstOrDefault(s => s.ProcessorId == paySubscriptionResult.PaySubscription.ProcessorId);
        if (paySubscription is not null)
        {
            // If pause behavior is changing to `void`, record the pause start date
            // Any other pause status (or no pause at all) should have nil for start
            if (paySubscription.PauseBehaviour != paySubscriptionResult.PaySubscription.PauseBehaviour &&
                paySubscriptionResult.PaySubscription.PauseBehaviour == PaySubscriptionPauseBehaviour.Void)
            {
                paySubscriptionResult.PaySubscription.PauseStartsAt = paySubscriptionResult.PaySubscription.CurrentPeriodEnd;
            }
            await _subscriptionStore.UpdateAsync(paySubscriptionResult.PaySubscription);
        }
        else
        {
            await _subscriptionStore.CreateAsync(paySubscriptionResult.PaySubscription);
        }

        // Sync the latest charge if we already have it loaded (like during subscrbe), otherwise, let webhooks take care of creating it
        PayCharge? charge = paySubscriptionResult.PaySubscription.Charges.LastOrDefault();
        if (charge is not null && charge.Status == PayStatus.Succeeded)
        {
            await _chargeManager.SynchroniseAsync(payCustomer, charge.ProcessorId);
        }

        return paySubscriptionResult.PaySubscription;
    }
}