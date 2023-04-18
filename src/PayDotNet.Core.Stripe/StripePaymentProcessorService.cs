using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;
using Stripe;

namespace PayDotNet.Core.Stripe;

public class StripePaymentProcessorService : IPaymentProcessorService
{
    private readonly CustomerService _customers;
    private readonly SubscriptionService _subscriptions;
    private readonly PaymentMethodService _paymentMethods;

    public StripePaymentProcessorService()
    {
        _customers = new CustomerService();
        _subscriptions = new SubscriptionService();
        _paymentMethods = new PaymentMethodService();
    }

    public async Task<PaymentProcessorCustomer> CreateCustomerAsync(string email, Dictionary<string, string> attributes)
    {
        Customer customer = await _customers.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Expand = new() { "tax" }
        });

        return new PaymentProcessorCustomer(customer.Id, new());
    }

    public async Task<PaymentProcessorSubscription?> GetSubscriptionAsync(string subscriptionId)
    {
        Subscription subscription = await _subscriptions.GetAsync(subscriptionId);
        if (subscription == null)
        {
            return null;
        }
        return Map(subscription);
    }

    public async Task<PaymentProcessorSubscription> CreateSubscriptionAsync(PayCustomer customer, string plan, Dictionary<string, object?> attributes)
    {
        Subscription subscription = await _subscriptions.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customer.ProcessorId,
            Expand = new() { "pending_setup_intent", "latest_invoice.payment_intent", "latest_invoice.charge" },
            Items = new()
            {
                new()
                {
                    Price = plan,
                    Quantity = 1
                }
            },
            PaymentSettings = new()
            {
                SaveDefaultPaymentMethod = "on_subscription",
            },
            PaymentBehavior = "default_incomplete"
        });

        return Map(subscription);
    }

    public Task<PaymentProcessorCustomer?> GetCustomerAsync(string processorId)
    {
        throw new NotImplementedException();
    }

    private static StripePaymentProcessorSubscription Map(Subscription subscription)
    {
        return new StripePaymentProcessorSubscription(subscription.Id, subscription.Customer.Id, new Dictionary<string, object?>
        {
            ["application_fee_percent"] = subscription.ApplicationFeePercent,
            ["created_at"] = subscription.Created,
            ["processor_plan"] = subscription.Items.First().Price.Id,
            ["quantity"] = subscription.Items.First().Quantity,
            ["status"] = subscription.Status,
            //["stripe_account"] = payCustomer.StripeAccount,
            ["metadata"] = subscription.Metadata,
            ["subscription_items"] = new string[0],
            ["is_metered"] = false,
            ["pause_behavior"] = subscription.PauseCollection.Behavior,
            ["pause_resumes_at"] = subscription.PauseCollection.ResumesAt,
            ["current_period_start"] = subscription.CurrentPeriodStart,
            ["current_period_end"] = subscription.CurrentPeriodEnd,
        }, null);
    }

    public async Task<PaymentProcessorPaymentMethod> AttachPaymentMethodAsync(string processorId, string paymentMethodId, bool isDefault)
    {
        PaymentMethod paymentMethod = await _paymentMethods.AttachAsync(paymentMethodId, new()
        {
            Customer = processorId,
        });

        if (isDefault)
        {
            await _customers.UpdateAsync(processorId, new CustomerUpdateOptions
            {
                InvoiceSettings = new()
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            });
        }

        return new PaymentProcessorPaymentMethod(paymentMethod.Id, paymentMethod.Type, isDefault);
    }

    public async Task<PaymentProcessorCustomer> FindCustomerAsync(string processorId)
    {
        Customer customer = await _customers.GetAsync(processorId, new CustomerGetOptions
        {
            Expand = new() { "tax" }
        });
        return new PaymentProcessorCustomer(customer.Id, new());
    }
}