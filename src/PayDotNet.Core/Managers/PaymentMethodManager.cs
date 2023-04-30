using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class PaymentMethodManager : IPaymentMethodManager
{
    private readonly IPaymentMethodStore _paymentMethodStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public PaymentMethodManager(
        IPaymentMethodStore paymentMethodStore,
        IPaymentProcessorService paymentProcessorService)
    {
        _paymentMethodStore = paymentMethodStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public async Task<PayPaymentMethod> AddPaymentMethodAsync(PaymentProcessorCustomer customer, string processorId, bool isDefault = false)
    {
        PaymentProcessorPaymentMethod paymentProcessorPaymentMethod =
            await _paymentProcessorService.AttachPaymentMethodAsync(customer.Id, processorId, isDefault);

        PayPaymentMethod paymentMethod = await SavePaymentMethodAsync(isDefault, paymentProcessorPaymentMethod);
        return paymentMethod;
    }

    public Task DeleteAllAsync(PayCustomer payCustomer)
    {
        return Task.CompletedTask;
    }

    public Task DeleteByIdAsync(string processorName, string processorId)
    {
        return Task.CompletedTask;
    }

    public bool IsPaymentMethodRequired() => _paymentProcessorService.IsPaymentMethodRequired;

    public Task<PayPaymentMethod> SynchroniseAsync(string processorId)
    {
        return Task.FromResult<PayPaymentMethod>(new());
    }

    public Task UpdateAllAsync(bool isDefault)
    {
        return Task.CompletedTask;
    }

    private async Task<PayPaymentMethod> SavePaymentMethodAsync(bool isDefault, PaymentProcessorPaymentMethod paymentProcessorPaymentMethod)
    {
        PayPaymentMethod paymentMethod = new()
        {
            ProcessorId = paymentProcessorPaymentMethod.Id,
            IsDefault = isDefault,
            Type = paymentProcessorPaymentMethod.Type
        };
        await _paymentMethodStore.CreateAsync(paymentMethod);
        return paymentMethod;
    }
}