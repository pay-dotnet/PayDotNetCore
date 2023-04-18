using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly IPaymentProcessorService _paymentProcessorService;
    private readonly ISubscriptionStore _subscriptionStore;
    private readonly ICustomerStore _customerStore;

    public SubscriptionManager(IPaymentProcessorService paymentProcessorService,
        ISubscriptionStore subscriptionStore,
        ICustomerStore customerStore)
    {
        _paymentProcessorService = paymentProcessorService;
        _subscriptionStore = subscriptionStore;
        _customerStore = customerStore;
    }

    public Task<PaySubscription> CreateAsync(string name, string processorId, string processorPlan, PayStatus status, DateTime? trailEndsAt, Dictionary<string, object?> Metadata)
    {
        throw new NotImplementedException();
    }

    public async Task<PaySubscription> SynchroniseAsync(string subscriptionId, object @object = null, string name = "", string stripeAcount = "", int attempt = 0, int retries = 1)
    {
        PaymentProcessorSubscription paymentProcessorSubscription = await _paymentProcessorService.GetSubscriptionAsync(subscriptionId);

        PayCustomer? customer = _customerStore.Customers.FirstOrDefault(c => c.Processor == "TODO" && c.ProcessorId == paymentProcessorSubscription.CustomerId.ToString());

        // https://github.com/pay-rails/pay/blob/v6.3.1/lib/pay/stripe/subscription.rb
        throw new NotImplementedException();
    }
}
