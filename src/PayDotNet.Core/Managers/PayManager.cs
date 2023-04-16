using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Infrastructure;

namespace PayDotNet.Core.Managers;

public class PayManager : IPayManager
{
    private readonly IModelStore<PayCustomer> _customerStore;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public PayManager(
        ISubscriptionManager subscriptionManager,
        IPaymentProcessorService paymentProcessorService,
        IModelStore<PayCustomer> customerStore
        )
    {
        _subscriptionManager = subscriptionManager;
        _paymentProcessorService = paymentProcessorService;
        _customerStore = customerStore;
    }

    public async Task<PayCustomer> ResolveCustomerAsync(PayCustomer customer)
    {
        if (!customer.HasProcessorId())
        {
            // Create customer
            PaymentProcessorCustomer result = await _paymentProcessorService.CreateCustomerAsync(customer.Email, new());

            // Update processorId
            customer.ProcessorId = result.Id;
            await _customerStore.UpdateAsync(customer.Id, customer);

        }
        return customer;
    }

    public async Task<PaySubscription> SubscribeAsync(PayCustomer customer, string name = "default", string plan = "default")
    {
        PayPaymentMethod? paymentMethod = customer.PaymentMethods.FirstOrDefault(p => p.IsDefault);
        if (paymentMethod == null)
        {
            throw new PayDotNetException("Customer has no default payment method");
        }

        // TODO: Standardize the trial period options
        //# Standardize the trial period options
        //if (trial_period_days = options.delete(:trial_period_days)) && trial_period_days > 0
        //  options.merge!(trial_period: true, trial_duration: trial_period_days, trial_duration_unit: :day)
        //end

        // TODO: customer.Id could also be Processor.Id
        PaymentProcessorSubscription paymentProcessorSubscription = await _paymentProcessorService.CreateSubscriptionAsync(customer.Id, new());
        // BrainTree vs Stripe logic is different

        PaySubscription subscription = await _subscriptionManager.CreateAsync(name, paymentProcessorSubscription.Id, plan, PayStatus.Active, paymentProcessorSubscription.GetTrialEndDate(), paymentProcessorSubscription.Attributes);
        return subscription;
    }
}
