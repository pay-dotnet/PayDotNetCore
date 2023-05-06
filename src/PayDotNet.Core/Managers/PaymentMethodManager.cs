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

    public virtual async Task<PayPaymentMethod> AddPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options)
    {
        PayPaymentMethod payPaymentMethod = await _paymentProcessorService.AttachPaymentMethodAsync(payCustomer, options);

        await SavePaymentMethodAsync(payPaymentMethod);
        return payPaymentMethod;
    }

    public virtual Task DeleteAllAsync(PayCustomer payCustomer)
    {
        throw new NotImplementedException();
    }

    public virtual Task DeleteByIdAsync(string processorName, string processorId)
    {
        throw new NotImplementedException();
    }

    public virtual bool IsPaymentMethodRequired(PayCustomer payCustomer) => _paymentProcessorService.IsPaymentMethodRequired(payCustomer);

    public virtual Task<PayPaymentMethod> SynchroniseAsync(string processorId)
    {
        return Task.FromResult<PayPaymentMethod>(new());
    }

    public virtual Task UpdateAllAsync(bool isDefault)
    {
        throw new NotImplementedException();
    }

    private async Task SavePaymentMethodAsync(PayPaymentMethod payPaymentMethod)
    {
        PayPaymentMethod? existingPaymentMethod = _paymentMethodStore.PaymentMethods.FirstOrDefault(p => p.ProcessorId == payPaymentMethod.ProcessorId);
        if (existingPaymentMethod is null)
        {
            await _paymentMethodStore.CreateAsync(payPaymentMethod);
        }
        else
        {
            // Ignore the payment method if it's already in the database
            if (payPaymentMethod.IsDefault)
            {
                ICollection<PayPaymentMethod> otherPaymentMethods = _paymentMethodStore.PaymentMethods
                    .Where(p => p.CustomerId == payPaymentMethod.CustomerId && p.ProcessorId != payPaymentMethod.ProcessorId)
                    .ToList();
                foreach (var otherPaymentMethod in otherPaymentMethods)
                {
                    otherPaymentMethod.IsDefault = false;
                }
                await _paymentMethodStore.UpdateAsync(otherPaymentMethods);
            }

            existingPaymentMethod.IsDefault = true;
            await _paymentMethodStore.UpdateAsync(existingPaymentMethod);
        }
    }
}