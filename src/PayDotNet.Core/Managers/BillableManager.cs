using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class BillableManager : IBillableManager
{
    private readonly ICustomerManager _customerManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public BillableManager(
        ICustomerManager customerManager,
        ISubscriptionManager subscriptionManager,
        IPaymentProcessorService paymentProcessorService
        )
    {
        _customerManager = customerManager;
        _subscriptionManager = subscriptionManager;
        _paymentProcessorService = paymentProcessorService;
    }

    // https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
    public async Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName, bool allowFake = false)
    {
        if (processorName.Equals(PaymentProcessors.Fake, StringComparison.OrdinalIgnoreCase) && !allowFake)
        {
            throw new PayDotNetException($"Processor `{processorName}` is not allowed");
        }

        return await _customerManager.GetOrCreateCustomerAsync(email, processorName);
    }

    public async Task<PaySubscription> SubscribeAsync(PayCustomer payCustomer, string name = "default", string price = "default")
    {
        if (_paymentProcessorService.IsPaymentMethodRequired)
        {
            PayPaymentMethod? paymentMethod = payCustomer.PaymentMethods.FirstOrDefault(p => p.IsDefault);
            if (paymentMethod == null)
            {
                throw new PayDotNetException("Customer has no default payment method");
            }
        }

        PaySubscriptionResult result = await _subscriptionManager.CreateSubscriptionAsync(payCustomer, price);
        if (result.Payment is not null)
        {
            result.Payment.Validate();
        }

        return result.PaySubscription;
    }
}