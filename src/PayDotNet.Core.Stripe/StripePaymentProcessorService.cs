using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Infrastructure;
using Stripe;

namespace PayDotNet.Core.Stripe;


public class StripePaymentProcessorService : IPaymentProcessorService
{
    public StripePaymentProcessorService()
    {
    }


    public async Task<PaymentProcessorCustomer> CreateCustomerAsync(string email, Dictionary<string, string> attributes)
    {
        var service = new CustomerService();
        Customer customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Expand = new() { "tax" }
        });

        return new PaymentProcessorCustomer(customer.Id, new());
    }

    public async Task<PaymentProcessorSubscription?> GetSubscriptionAsync(string subscriptionId)
    {
        var service = new SubscriptionService();
        Subscription subscription = await service.GetAsync(subscriptionId);
        if (subscription == null)
        {
            return null;
        }
        return Map(subscription);
    }

    public async Task<PaymentProcessorSubscription> CreateSubscriptionAsync(PayCustomer customer, string plan, Dictionary<string, object?> attributes)
    {
        var service = new SubscriptionService();
        Subscription subscription = await service.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customer.ProcessorId,
            Expand = new() { "pending_setup_intent", "latest_invoice.payment_intent", "latest_invoice.charge" },
            Items = new()
            {
                new()
                {
                    Plan = plan,
                    Quantity = 1
                }
            }
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
        });
    }

    public async Task<PaymentProcessorPaymentMethod> CreatePaymentMethodAsync(string processorId, string paymentMethodId, bool isDefault)
    {
        PaymentMethodService paymentMethodService = new();
        PaymentMethod paymentMethod = await paymentMethodService.AttachAsync(paymentMethodId, new()
        {
            Customer = processorId,
        });

        CustomerService customerService = new();

        if (isDefault)
        {
            await customerService.UpdateAsync(processorId, new CustomerUpdateOptions
            {
                InvoiceSettings = new()
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            });
        }

        return new PaymentProcessorPaymentMethod(paymentMethod.Id, paymentMethod.Type, isDefault);
    }
}


public record StripePaymentProcessorSubscription(
    string Id,
    string CustomerId,
    Dictionary<string, object?> Attributes,
    IPayment Payment) : PaymentProcessorSubscription(Id, CustomerId, Attributes, Payment)
{
    public override DateTime? GetTrialEndDate()
    {
        throw new NotImplementedException();
    }
}