using Microsoft.Extensions.Options;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;
using PayDotNet.Core.Stripe.Client;
using Stripe;

namespace PayDotNet.Core.Stripe;

public class StripePaymentProcessorService : IPaymentProcessorService
{
    private readonly PaymentIntentService _paymentIntents;
    private readonly ChargeService _charges;
    private readonly CustomerService _customers;
    private readonly SubscriptionService _subscriptions;
    private readonly PaymentMethodService _paymentMethods;
    private readonly SetupIntentService _setupIntents;
    private readonly IOptions<PayDotNetConfiguration> _options;
    private readonly DataTransferObjectMapper _mapper;

    public bool IsPaymentMethodRequired => false;

    public StripePaymentProcessorService(
        IOptions<PayDotNetConfiguration> options)
    {
        _charges = new();
        _customers = new();
        _subscriptions = new();
        _paymentMethods = new();
        _paymentIntents = new();
        _setupIntents = new();
        _options = options;
        _mapper = new(options);
    }

    private static readonly List<string> DefaultSubscriptionExpandOptions = new()
    {
        "pending_setup_intent",
        "latest_invoice.payment_intent",
        "latest_invoice.charge"
    };

    public async Task<PaymentProcessorCustomer> CreateCustomerAsync(string email, Dictionary<string, string> attributes)
    {
        Customer customer = await _customers.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Expand = new() { "tax" }
        });

        return new PaymentProcessorCustomer(customer.Id, new());
    }

    public async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string[] plans, Dictionary<string, object?> attributes)
    {
        SubscriptionCreateOptions options = new SubscriptionCreateOptions
        {
            Customer = payCustomer.ProcessorId,
            Items = plans.Select(p => new SubscriptionItemOptions()
            {
                Price = p,
                Quantity = 1
            }).ToList(),
            PaymentBehavior = _options.Value.Stripe.PaymentBehaviour,
            Metadata = new()
            {
                ["pay_name"] = _options.Value.DefaultPlanName
            },
        };

        options.AddExpand("latest_invoice");
        options.AddExpand("latest_invoice.payment_intent");
        options.AddExpand("latest_invoice.charge");
        options.AddExpand("pending_setup_intent");

        Subscription subscription = await _subscriptions.CreateAsync(options);
        return Map(subscription);
    }

    public Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string plan, Dictionary<string, object?> attributes)
    {
        return CreateSubscriptionAsync(payCustomer, new string[] { plan }, attributes);
    }

    public async Task<PaySubscriptionResult?> GetSubscriptionAsync(string processorId, PayCustomer payCustomer)
    {
        Subscription? subscription = await _subscriptions.GetAsync(processorId, new()
        {
            Expand = DefaultSubscriptionExpandOptions,
        });
        if (subscription is null)
        {
            return null;
        }
        return Map(subscription);
    }

    private PaySubscriptionResult Map(Subscription @object)
    {
        PaySubscription paySubscription = _mapper.Map(@object);

        IPayment? payment = @object.LatestInvoice?.PaymentIntent is not null
            ? new StripePaymentIntentPayment(@object.LatestInvoice.PaymentIntent)
            : null;

        return new(paySubscription, payment);
    }

    public Task<PaymentProcessorCustomer?> GetCustomerAsync(string processorId)
    {
        throw new NotImplementedException();
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

    public async Task<PayCharge> GetChargeAsync(string processorId)
    {
        Charge charge = await _charges.GetAsync(processorId, new()
        {
            Expand = new()
            {
                "invoice.total_discount_amounts.discount",
                "invoice.total_tax_amounts.tax_rate",
                "refunds"
            }
        });
        return _mapper.Map(charge);
    }

    public async Task<IPayment> GetPaymentAsync(string processorId)
    {
        PaymentIntent paymentIntent;
        SetupIntent setupIntent;
        return processorId.StartsWith("seti_")
            ? new StripeSetupIntentPayment(await _setupIntents.GetAsync(processorId))
            : new StripePaymentIntentPayment(await _paymentIntents.GetAsync(processorId));
        throw new NotImplementedException();
    }
}

public static class DictionaryExtensions
{
    public static string? Try(this Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }
        return null;
    }
}