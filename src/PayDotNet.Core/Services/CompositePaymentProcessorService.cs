using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Services;

public class CompositePaymentProcessorService : IPaymentProcessorService
{
    private readonly Dictionary<string, IPaymentProcessorService> _paymentProcessorServices;

    public string Name => "Composite";

    protected virtual void GuardPaymentProcessorExists(PayCustomer payCustomer)
    {
        if (!_paymentProcessorServices.ContainsKey(payCustomer.Processor))
        {
            throw new PayDotNetException($"A {nameof(IPaymentProcessorService)} for the processor '{payCustomer.Processor}' was not registered");
        }
    }

    public CompositePaymentProcessorService(IEnumerable<IPaymentProcessorService> paymentProcessorServices)
    {
        _paymentProcessorServices = paymentProcessorServices
            .Where(p => p is not CompositePaymentProcessorService)
            .ToDictionary(service => service.Name);
    }

    public bool IsPaymentMethodRequired(PayCustomer payCustomer)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].IsPaymentMethodRequired(payCustomer);
    }

    public Task<PayPaymentMethod> AttachPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].AttachPaymentMethodAsync(payCustomer, options);
    }

    public Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].CancelAsync(payCustomer, paySubscription, options);
    }

    public Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].CaptureAsync(payCustomer, payCharge, options);
    }

    public Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].ChargeAsync(payCustomer, options);
    }

    public Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].CheckoutAsync(payCustomer, options);
    }

    public Task<string> CreateCustomerAsync(PayCustomer payCustomer)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].CreateCustomerAsync(payCustomer);
    }

    public Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].CreateSubscriptionAsync(payCustomer, options);
    }

    public Task<PayCharge> GetChargeAsync(PayCustomer payCustomer, string processorId)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].GetChargeAsync(payCustomer, processorId);
    }

    public Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].GetCreditNotesAsync(payCustomer, payCharge);
    }

    public Task<IPayment> GetPaymentAsync(PayCustomer payCustomer, string processorId)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].GetPaymentAsync(payCustomer, processorId);
    }

    public Task<PaySubscriptionResult?> GetSubscriptionAsync(PayCustomer payCustomer, string processorId)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].GetSubscriptionAsync(payCustomer, processorId);
    }

    public Task IssueCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].IssueCreditNotesAsync(payCustomer, payCharge, options);
    }

    public Task<PayChargeRefund> RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        GuardPaymentProcessorExists(payCustomer);
        return _paymentProcessorServices[payCustomer.Processor].RefundAsync(payCustomer, payCharge, options);
    }
}
