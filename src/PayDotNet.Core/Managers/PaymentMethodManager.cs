using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class PaymentMethodManager : IPaymentMethodManager
{
    private readonly ICustomerManager _customerManager;
    private readonly IPaymentMethodStore _paymentMethodStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public PaymentMethodManager(
        ICustomerManager customerManager,
        IPaymentMethodStore paymentMethodStore,
        IPaymentProcessorService paymentProcessorService)
    {
        _customerManager = customerManager;
        _paymentMethodStore = paymentMethodStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public async Task<PayPaymentMethod> AddPaymentMethodAsync(PaymentProcessorCustomer customer, string paymentMethodId, bool isDefault = false)
    {
        PaymentProcessorPaymentMethod paymentProcessorPaymentMethod =
            await _paymentProcessorService.AttachPaymentMethodAsync(customer.Id, paymentMethodId, isDefault);

        PayPaymentMethod paymentMethod = await SavePaymentMethodAsync(isDefault, paymentProcessorPaymentMethod);
        return paymentMethod;
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