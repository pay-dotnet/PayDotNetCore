using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class ChargeManager : IChargeManager
{
    private readonly ICustomerManager _customerManager;
    private readonly IChargeStore _chargeStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public ChargeManager(
        ICustomerManager customerManager,
        IChargeStore chargeStore,
        IPaymentProcessorService paymentProcessorService)
    {
        _customerManager = customerManager;
        _chargeStore = chargeStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public async Task<IPayment> CaptureAsync(PayCharge payCharge, PayChargeOptions options)
    {
        return await _paymentProcessorService.CaptureAsync(payCharge, options);
    }

    public async Task<IPayment> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        PayChargeResult result = await _paymentProcessorService.ChargeAsync(payCustomer, options);

        await SynchroniseAsync(payCustomer, result.PayCharge.ProccesorId, result.PayCharge);

        return result.Payment;
    }

    public Task<PayCharge?> GetAsync(string processorId)
    {
        return Task.FromResult(_chargeStore.Charges.FirstOrDefault(c => c.ProccesorId == processorId));
    }

    public Task<ICollection<PayChargeRefund>> GetCreditNotesAsync(PayCharge payCharge)
    {
        if (string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            throw new PayDotNetException("No InvoiceId on PayCharge");
        }
        // TODO:
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task RefundAsync(PayCharge payCharge, PayChargeRefundOptions options)
    {
        // Issues a CreditNote if there's an invoice, otherwise uses a Refund.
        // This allows Tax to be handled properly
        if (string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            await _paymentProcessorService.RefundAsync(payCharge, options);
        }
        else
        {
            await _paymentProcessorService.IssueCreditNotesAsync(payCharge, options);
        }
    }

    public async Task SynchroniseAsync(string processor, string processorId, string customerProcessorId, int attempt = 0, int retries = 1)
    {
        PayCharge payCharge = await _paymentProcessorService.GetChargeAsync(processorId);
        PayCustomer payCustomer = await _customerManager.FindByIdAsync(processor, customerProcessorId);
        await SynchroniseAsync(payCustomer, processorId, payCharge);
    }

    private async Task SynchroniseAsync(PayCustomer payCustomer, string processorId, PayCharge? @object)
    {
        @object ??= await _paymentProcessorService.GetChargeAsync(processorId);
        if (@object == null)
        {
            return;
        }

        PayCharge? existingCharge = _chargeStore.Charges.FirstOrDefault(s => s.ProccesorId == processorId);
        @object.CustomerId = payCustomer.Id;
        if (existingCharge is null)
        {
            await _chargeStore.CreateAsync(@object);
        }
        else
        {
            await _chargeStore.UpdateAsync(@object);
        }
    }
}