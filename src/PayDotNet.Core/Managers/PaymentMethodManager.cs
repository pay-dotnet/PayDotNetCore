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
        CompositePaymentProcessorService paymentProcessorService)
    {
        _paymentMethodStore = paymentMethodStore;
        _paymentProcessorService = paymentProcessorService;
    }

    /// <inheritdoc/>
    public virtual async Task<PayPaymentMethod> AddPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options)
    {
        PayPaymentMethod payPaymentMethod = await _paymentProcessorService.AttachPaymentMethodAsync(payCustomer, options);

        await SavePaymentMethodAsync(payPaymentMethod, options);
        return payPaymentMethod;
    }

    public virtual Task DeleteAllAsync(PayCustomer payCustomer)
    {
        ICollection<PayPaymentMethod> payPaymentMethods =
            _paymentMethodStore.PaymentMethods.Where(pm => pm.CustomerId == payCustomer.Id).ToList();
        return _paymentMethodStore.DeleteAllAsync(payPaymentMethods);
    }

    public virtual async Task DeleteByIdAsync(string processorName, string processorId)
    {
        PayPaymentMethod? payPaymentMethod = _paymentMethodStore.PaymentMethods.FirstOrDefault(pm => pm.ProcessorId == processorId);
        if (payPaymentMethod is not null)
        {
            await _paymentMethodStore.DeleteAsync(payPaymentMethod);
        }
    }

    public virtual bool IsPaymentMethodRequired(PayCustomer payCustomer) => _paymentProcessorService.IsPaymentMethodRequired(payCustomer);

    public virtual Task SynchroniseAsync(PayCustomer payCustomer, string processorId)
    {
        return SynchroniseAsync(payCustomer, processorId, null);
    }

    private async Task SynchroniseAsync(PayCustomer payCustomer, string processorId, PayPaymentMethod? @object)
    {
        @object ??= await _paymentProcessorService.GetPaymentMethodAsync(payCustomer, processorId);
        if (@object == null)
        {
            return;
        }

        PayPaymentMethod? existingPayPaymentMethod = _paymentMethodStore.PaymentMethods.FirstOrDefault(s => s.ProcessorId == processorId);
        @object.CustomerId = payCustomer.Id;
        if (existingPayPaymentMethod is null)
        {
            await _paymentMethodStore.CreateAsync(@object);
        }
        else
        {
            await _paymentMethodStore.UpdateAsync(@object);
        }
    }

    public virtual async Task UpdateAllAsync(PayCustomer payCustomer, bool isDefault)
    {
        ICollection<PayPaymentMethod> payPaymentMethods =
            _paymentMethodStore.PaymentMethods.Where(pm => pm.CustomerId == payCustomer.Id).ToList();
        foreach (var payPaymentMethod in payPaymentMethods)
        {
            payPaymentMethod.IsDefault = isDefault;
        }

        await _paymentMethodStore.UpdateAllAsync(payPaymentMethods);
    }

    private async Task SavePaymentMethodAsync(PayPaymentMethod payPaymentMethod, PayPaymentMethodOptions options)
    {
        PayPaymentMethod? existingPaymentMethod = _paymentMethodStore.PaymentMethods.FirstOrDefault(p => p.ProcessorId == payPaymentMethod.ProcessorId);
        if (existingPaymentMethod is null)
        {
            await _paymentMethodStore.CreateAsync(payPaymentMethod);
        }
        else
        {
            if (options.IsDefault)
            {
                await ResetDefaultPaymentMethodsAsync(payPaymentMethod);
            }

            existingPaymentMethod.IsDefault = options.IsDefault;
            await _paymentMethodStore.UpdateAsync(existingPaymentMethod);
        }
    }

    private async Task ResetDefaultPaymentMethodsAsync(PayPaymentMethod payPaymentMethod)
    {
        ICollection<PayPaymentMethod> otherPaymentMethods = _paymentMethodStore.PaymentMethods
            .Where(p => p.CustomerId == payPaymentMethod.CustomerId && p.ProcessorId != payPaymentMethod.ProcessorId)
            .ToList();
        foreach (var otherPaymentMethod in otherPaymentMethods)
        {
            otherPaymentMethod.IsDefault = false;
        }

        await _paymentMethodStore.UpdateAllAsync(otherPaymentMethods);
    }
}