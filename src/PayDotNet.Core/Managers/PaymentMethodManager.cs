﻿using PayDotNet.Core.Abstraction;
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

    public async Task<PayPaymentMethod> AddPaymentMethodAsync(PayCustomer payCustomer, string paymentMethodId, bool isDefault = false)
    {
        PayPaymentMethod payPaymentMethod =
            await _paymentProcessorService.AttachPaymentMethodAsync(payCustomer, paymentMethodId, isDefault);

        await SavePaymentMethodAsync(payPaymentMethod);
        return payPaymentMethod;
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