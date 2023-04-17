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

public class PaymentMethodManager : IPaymentMethodManager
{
    private readonly IModelStore<PayPaymentMethod> _paymentMethodStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public PaymentMethodManager(
        IModelStore<PayPaymentMethod> paymentMethodStore,
        IPaymentProcessorService paymentProcessorService)
    {
        _paymentMethodStore = paymentMethodStore;
        _paymentProcessorService = paymentProcessorService;
    }

    public async Task<PayPaymentMethod> CreateAsync(PayCustomer customer, string paymentMethodId, bool isDefault = false)
    {
        if (!customer.HasProcessorId())
        {
            throw new PayDotNetException($"Customer does exist in payment processor. Please make sure {nameof(BillableManager)}.{nameof(BillableManager.SetPaymentProcessorAsync)} was called");
        }

        PaymentProcessorPaymentMethod paymentProcessorPaymentMethod =
            await _paymentProcessorService.CreatePaymentMethodAsync(customer.ProcessorId, paymentMethodId, isDefault);

        PayPaymentMethod paymentMethod = new()
        {
            Customer = customer,
            CustomerId = customer.Id,
            ProcessorId = paymentProcessorPaymentMethod.Id,
            IsDefault = isDefault,
            Type = paymentProcessorPaymentMethod.Type
        };
        await _paymentMethodStore.CreateAsync(paymentMethod);
        return paymentMethod;
    }
}

public class CustomerManager : ICustomerManager
{
    private readonly ICustomerStore _customerStore;

    public CustomerManager(ICustomerStore customerStore)
    {
        _customerStore = customerStore;
    }

    public async Task<PayCustomer> CreateIfNotExistsAsync(string id, string email, string processorName)
    {
        PayCustomer? customer = _customerStore.Customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            customer = new()
            {
                Id = id,
                Email = email,
                Processor = processorName
            };
            await _customerStore.CreateAsync(customer);
        }
        // TODO: update.
        // https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
        return customer;
    }

    public Task<PayCustomer?> FindByEmailAsync(string email, string processorName)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Email == email && c.Processor == processorName));
    }

    public Task<PayCustomer?> FindByIdAsync(string id, string processorName)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Id == id && c.Processor == processorName));
    }

    public Task UpdateAsync(PayCustomer customer)
    {
        return _customerStore.UpdateAsync(customer.Id, customer);
    }
}