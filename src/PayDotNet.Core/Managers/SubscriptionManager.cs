using Microsoft.Extensions.Options;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly IChargeManager _chargeManager;
    private readonly ISubscriptionStore _subscriptionStore;
    private readonly IPaymentProcessorService _paymentProcessorService;
    private readonly IOptions<PayDotNetConfiguration> _options;

    public SubscriptionManager(
        IChargeManager chargeManager,
        ISubscriptionStore subscriptionStore,
        CompositePaymentProcessorService paymentProcessorService,
        IOptions<PayDotNetConfiguration> options)
    {
        _chargeManager = chargeManager;
        _subscriptionStore = subscriptionStore;
        _paymentProcessorService = paymentProcessorService;
        _options = options;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The flow might be different for other payment processor. Stripe and Braintree work a bit differently.
    /// TODO: Resolve the above.
    /// </remarks>
    public virtual async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        PaySubscriptionResult result = await _paymentProcessorService.CreateSubscriptionAsync(payCustomer, options);
        await SynchroniseAsync(payCustomer, result);
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

        return _subscriptionStore.UpdateAllAsync(customerSubscriptions);
    }

    /// <inheritdoc/>
    public async Task SynchroniseAsync(PayCustomer payCustomer, string processorId)
    {
        PaySubscriptionResult? result = await _paymentProcessorService.GetSubscriptionAsync(payCustomer, processorId);
        if (result is null)
        {
            return;
        }
        await SynchroniseAsync(payCustomer, result);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// TODO: there should be a retry mechanism, but maybe we can exclude that and use Polly or tell people to use polly and document it.
    /// </remarks>
    public async Task SynchroniseAsync(PayCustomer payCustomer, PaySubscriptionResult result)
    {
        // Fix link to Pay Customer
        result.PaySubscription.CustomerId = payCustomer.Id;

        PaySubscription? paySubscription = _subscriptionStore.Subscriptions.FirstOrDefault(s => s.ProcessorId == result.PaySubscription.ProcessorId);
        if (paySubscription is not null)
        {
            // If pause behavior is changing to `void`, record the pause start date
            // Any other pause status (or no pause at all) should have nil for start
            if (paySubscription.PauseBehaviour != result.PaySubscription.PauseBehaviour && result.PaySubscription.PauseBehaviour == "void")
            {
                result.PaySubscription.PauseStartsAt = result.PaySubscription.CurrentPeriodEnd;
            }
            await _subscriptionStore.UpdateAsync(result.PaySubscription);
        }
        else
        {
            // Allow setting the subscription name in metadata, otherwise use the default
            result.PaySubscription.Name = result.PaySubscription.Metadata.TryOrDefault(PayMetadata.Fields.PaySubscriptionName, _options.Value.DefaultProductName);
            await _subscriptionStore.CreateAsync(result.PaySubscription);
        }

        // Sync the latest charge if we already have it loaded (like during subscrbe), otherwise, let webhooks take care of creating it
        PayCharge? charge = result.PaySubscription.Charges.LastOrDefault();
        if (charge is not null && charge.Status == PayStatus.Succeeded)
        {
            PayCharge? _ = await _chargeManager.SynchroniseAsync(payCustomer, charge.ProcessorId);
        }
    }
}