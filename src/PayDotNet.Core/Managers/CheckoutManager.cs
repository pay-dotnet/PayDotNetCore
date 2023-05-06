using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class CheckoutManager : ICheckoutManager
{
    private readonly IPaymentProcessorService _paymentProcessorService;

    public CheckoutManager(CompositePaymentProcessorService paymentProcessorService)
    {
        _paymentProcessorService = paymentProcessorService;
    }

    public virtual Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        return _paymentProcessorService.CheckoutAsync(payCustomer, options);
    }
}