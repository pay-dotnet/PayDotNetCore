﻿using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class ChargeManager : IChargeManager
{
    private readonly IChargeStore _chargeStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public ChargeManager(
        IChargeStore chargeStore,
        CompositePaymentProcessorService paymentProcessorService)
    {
        _chargeStore = chargeStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public virtual async Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeOptions options)
    {
        return await _paymentProcessorService.CaptureAsync(payCustomer, payCharge, options);
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

    public virtual Task<PayCharge?> GetAsync(string processorId)
    {
        return Task.FromResult(_chargeStore.Charges.FirstOrDefault(c => c.ProcessorId == processorId));
    }

    public virtual Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        if (string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            throw new PayDotNetException("No InvoiceId on PayCharge");
        }
        return _paymentProcessorService.GetCreditNotesAsync(payCustomer, payCharge);
    }

    /// <inheritdoc/>
    public virtual async Task RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        // Issues a CreditNote if there's an invoice, otherwise uses a Refund.
        // This allows Tax to be handled properly
        if (string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            await _paymentProcessorService.RefundAsync(payCustomer, payCharge, options);
        }
        else
        {
            await _paymentProcessorService.IssueCreditNotesAsync(payCustomer, payCharge, options);
        }
    }

    /// <inheritdoc/>
    public virtual async Task SynchroniseAsync(PayCustomer payCustomer, string processorId)
    {
        PayCharge? payCharge = await _paymentProcessorService.GetChargeAsync(payCustomer, processorId);
        if (payCharge is null)
        {
            return;
        }
        await SynchroniseAsync(payCustomer, payCharge);
    }

    private async Task SynchroniseAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        // Fix link to Pay Customer
        payCharge.CustomerId = payCustomer.Id;

        PayCharge? existingCharge = _chargeStore.Charges.FirstOrDefault(s => s.ProcessorId == payCharge.ProcessorId);
        if (existingCharge is not null)
        {
            await _chargeStore.UpdateAsync(payCharge);
        }
        else
        {
            await _chargeStore.CreateAsync(payCharge);
        }
    }
}