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
        IPaymentProcessorService paymentProcessorService)
    {
        _chargeManager = chargeManager;
        _subscriptionStore = subscriptionStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public Task CancellAllAsync(PayCustomer payCustomer)
    {
        throw new NotImplementedException();
    }

    public async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string priceId, string name)
    {
        // TODO: BrainTree vs Stripe logic is different
        PaySubscriptionResult result = await _paymentProcessorService.CreateSubscriptionAsync(payCustomer, priceId, new());

        await SynchroniseAsync(result.PaySubscription.ProcessorId, result, payCustomer);

        return result;
    }

    public Task<PaySubscription?> FindByIdAsync(string processorId, string customerId)
    {
        return Task.FromResult(_subscriptionStore.Subscriptions.FirstOrDefault(s => s.ProcessorId == processorId && s.CustomerId == customerId));
    }

    public async Task SynchroniseAsync(string processorId, PaySubscriptionResult? @object, PayCustomer payCustomer)
    {
        @object ??= await _paymentProcessorService.GetSubscriptionAsync(processorId, payCustomer);
        if (@object == null)
        {
            return;
        }

        PaySubscription? existingPaySubscription = _subscriptionStore.Subscriptions.FirstOrDefault(s => s.ProcessorId == @object.PaySubscription.ProcessorId);

        @object.PaySubscription.CustomerId = payCustomer.Id;
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