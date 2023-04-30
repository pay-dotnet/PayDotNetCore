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