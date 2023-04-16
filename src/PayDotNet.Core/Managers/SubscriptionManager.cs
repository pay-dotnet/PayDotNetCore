using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Infrastructure;

namespace PayDotNet.Core.Managers;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly IPaymentProcessorService _paymentProcessorService;
    private readonly IModelStore<PaySubscription> _subscriptionStore;
    private readonly ICustomerStore _customerStore;

    public SubscriptionManager(IPaymentProcessorService paymentProcessorService,
        IModelStore<PaySubscription> subscriptionStore,
        ICustomerStore customerStore)
    {
        _paymentProcessorService = paymentProcessorService;
        _subscriptionStore = subscriptionStore;
        _customerStore = customerStore;
    }

    public Task<PaySubscription> CreateAsync(string name, string processorId, string processorPlan, PayStatus status, DateTime? trailEndsAt, Dictionary<string, string> Metadata)
    {
        throw new NotImplementedException();
    }

    public async Task<PaySubscription?> SynchroniseAsync(string subscriptionId, object @object = null, string name = "", string stripeAcount = "", int attempt = 0, int retries = 1)
    {
        PaymentProcessorSubscription paymentProcessorSubscription = await _paymentProcessorService.GetSubscriptionAsync(subscriptionId);

        PayCustomer? customer = _customerStore.Customers.FirstOrDefault(c => c.Processor == "TODO" && c.ProcessorId == paymentProcessorSubscription.Attributes["customer"].ToString());

        // https://github.com/pay-rails/pay/blob/v6.3.1/lib/pay/stripe/subscription.rb
        throw new NotImplementedException();
    }
}
