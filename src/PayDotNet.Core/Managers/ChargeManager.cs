using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class ChargeManager : IChargeManager
{
    private readonly IChargeStore _chargeStore;
    private readonly IPaymentProcessorService _paymentProcessorService;
    private readonly ISubscriptionManager _subscriptionManager;

    public ChargeManager(
        ISubscriptionManager subscriptionManager,
        IChargeStore chargeStore,
        CompositePaymentProcessorService paymentProcessorService)
    {
        _subscriptionManager = subscriptionManager;
        _chargeStore = chargeStore;
        _paymentProcessorService = paymentProcessorService;
    }

    /// <inheritdoc/>
    public virtual async Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeCaptureOptions options)
    {
        IPayment payment = await _paymentProcessorService.CaptureAsync(payCustomer, payCharge, options);
        await SynchroniseAsync(payCustomer, payCharge.ProcessorId);
        return payment;
    }

    /// <inheritdoc/>
    public virtual async Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        PayChargeResult result = await _paymentProcessorService.ChargeAsync(payCustomer, options);
        if (result.PayCharge is not null && result.Payment.IsSucceeded())
        {
            await SynchroniseAsync(payCustomer, result.PayCharge);
        }

        return result;
    }

    /// <inheritdoc/>
    public virtual Task<PayCharge?> GetAsync(string processorId)
    {
        return Task.FromResult(_chargeStore.Charges.FirstOrDefault(c => c.ProcessorId == processorId));
    }

    public Task<IPayment> GetPaymentAsync(PayCustomer payCustomer, string processorId)
    {
        return _paymentProcessorService.GetPaymentAsync(payCustomer, processorId);
    }

    /// <inheritdoc/>
    public virtual async Task RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        // Issues a CreditNote if there's an invoice, otherwise uses a Refund.
        // This allows Tax to be handled properly
        if (!string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            await _paymentProcessorService.IssueCreditNotesAsync(payCustomer, payCharge, options);
        }
        else
        {
            await _paymentProcessorService.RefundAsync(payCustomer, payCharge, options);
        }
    }

    /// <inheritdoc/>
    public virtual async Task<PayCharge> SynchroniseAsync(PayCustomer payCustomer, string processorId)
    {
        PayCharge? payCharge = await _paymentProcessorService.GetChargeAsync(payCustomer, processorId);
        if (payCharge is null)
        {
            throw new PayDotNetException(string.Format("Charge with identifier '{0}', does not exist. Unable to synchronise PayCharge", processorId));
        }
        return await SynchroniseAsync(payCustomer, payCharge);
    }

    private Task<PayCharge> SynchroniseAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        return Synchronizer.Retry(() => SynchroniseAsyncInternal(payCustomer, payCharge));
    }

    private async Task<PayCharge> SynchroniseAsyncInternal(PayCustomer payCustomer, PayCharge payCharge)
    {
        // Fix link to Pay Customer
        payCharge.CustomerId = payCustomer.Id;

        // Fix link to Pay Subscription
        if (!string.IsNullOrEmpty(payCharge.SubscriptionProcessorId))
        {
            PaySubscription? paySubscription = await _subscriptionManager.FindByIdAsync(payCustomer, payCharge.SubscriptionProcessorId);
            payCharge.Subscription = paySubscription;
        }

        PayCharge? existingCharge = _chargeStore.Charges.FirstOrDefault(s => s.ProcessorId == payCharge.ProcessorId);
        if (existingCharge is not null)
        {
            await _chargeStore.UpdateAsync(payCharge);
        }
        else
        {
            await _chargeStore.CreateAsync(payCharge);
        }
        return payCharge;
    }
}