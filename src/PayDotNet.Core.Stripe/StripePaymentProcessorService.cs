using Microsoft.Extensions.Options;
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

    public bool IsPaymentMethodRequired => false;

    public StripePaymentProcessorService(IOptions<PayDotNetConfiguration> options)
    {
        _customers = new CustomerService();
        _subscriptions = new SubscriptionService();
        _paymentMethods = new PaymentMethodService();
        _options = options;
    }

    private static readonly List<string> DefaultExpandOptions = new()
    {
        "pending_setup_intent",
        "latest_invoice.payment_intent",
        "latest_invoice.charge",
        "latest_invoice.total_discount_amounts.discount",
        "latest_invoice.total_tax_amounts.tax_rate"
    };

    private readonly IOptions<PayDotNetConfiguration> _options;

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
        // TODO: include pay_name as metadata.
        Subscription subscription = await _subscriptions.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = payCustomer.ProcessorId,
            Items = plans.Select(p => new SubscriptionItemOptions()
            {
                Price = p,
                Quantity = 1
            }).ToList(),
            PaymentSettings = new()
            {
                SaveDefaultPaymentMethod = "on_subscription",
            },
            PaymentBehavior = "default_incomplete",
            Expand = DefaultExpandOptions,
            Metadata = new()
            {
                ["pay_name"] = _options.Value.DefaultPlanName
            },
        });
        return Map(payCustomer, null, subscription);
    }

    public Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, string plan, Dictionary<string, object?> attributes)
    {
        return CreateSubscriptionAsync(payCustomer, new string[] { plan }, attributes);
    }

    public async Task<PaySubscriptionResult?> GetSubscriptionAsync(string processorId, PayCustomer payCustomer)
    {
        Subscription? subscription = await _subscriptions.GetAsync(processorId, new()
        {
            Expand = DefaultExpandOptions,
        });
        if (subscription is null)
        {
            return null;
        }
        return Map(payCustomer, null, subscription);
    }

    private PaySubscriptionResult Map(PayCustomer payCustomer, PaySubscription? existingPaySubscription, Subscription @object)
    {
        string name = @object.Metadata.ContainsKey("pay_name")
            ? @object.Metadata["pay_name"]
            : _options.Value.DefaultPlanName;

        StripePaymentProcessorSubscription paySubscription = new()
        {
            CustomerId = payCustomer.Id,

            Name = name,
            ApplicationFeePercent = @object.ApplicationFeePercent,
            CreatedAt = @object.Created,
            Processor = PaymentProcessors.Stripe,
            ProcessorId = @object.Id,
            ProcessorPlan = @object.Items.First().Price.Id,
            Quantity = Convert.ToInt32(@object.Items.First().Quantity),
            Status = ConvertStatus(@object),
            IsMetered = false,
            PauseBehaviour = @object.PauseCollection?.Behavior,
            PauseResumesAt = @object.PauseCollection?.ResumesAt,
            CurrentPeriodStart = @object.CurrentPeriodStart,
            CurrentPeriodEnd = @object.CurrentPeriodEnd,
            Metadata = @object.Metadata,
            Data = new(StringComparer.OrdinalIgnoreCase)
            {
                // TODO: ["stripe_account"] = payCustomer.StripeAccount
                ["payment_intent_id"] = @object.LatestInvoice.PaymentIntentId,
                ["client_secret"] = @object.LatestInvoice.PaymentIntent.ClientSecret,
            }
        };

        if (@object.TrialEnd.HasValue)
        {
            @object.TrialEnd = new[] { @object.EndedAt, @object.TrialEnd }.Min();
        }

        // Record subscription items to model
        foreach (var subscriptionItem in @object.Items)
        {
            if (!paySubscription.IsMetered && subscriptionItem.Price.Recurring.UsageType == "metered")
            {
                paySubscription.IsMetered = true;
            }

            var paySubscriptionItem = new { subscriptionItem.Id, subscriptionItem.Price, subscriptionItem.Metadata, subscriptionItem.Quantity };
            paySubscription.SubscriptionItems.Add(paySubscriptionItem);
        }

        if (@object.EndedAt.HasValue)
        {
            // Fully cancelled object
            paySubscription.EndsAt = @object.EndedAt.Value;
        }
        else if (@object.CancelAt.HasValue)
        {
            // Subscription cancelling in the future
            paySubscription.EndsAt = @object.CancelAt.Value;
        }
        else if (@object.CancelAtPeriodEnd)
        {
            // Subscriptions cancelling the future
            paySubscription.EndsAt = @object.CurrentPeriodEnd;
        }

        // If pause behavior is changing to `void`, record the pause start date
        // Any other pause status (or no pause at all) should have nil for start
        if (existingPaySubscription is not null)
        {
            if (existingPaySubscription.PauseBehaviour != paySubscription.PauseBehaviour && paySubscription.PauseBehaviour == "void")
            {
                existingPaySubscription.PauseStartsAt = existingPaySubscription.CurrentPeriodEnd;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(paySubscription.Name))
            {
                paySubscription.Name = @object.Metadata["pay_name"] ?? _options.Value.DefaultProductName;
            }
        }

        IPayment? payment = @object.LatestInvoice?.PaymentIntent is not null
            ? new StripePayment(@object.LatestInvoice.PaymentIntent)
            : null;

        return new(paySubscription, payment);
    }

    private static PaySubscriptionStatus ConvertStatus(Subscription subscription)
    {
        switch (subscription.Status)
        {
            case "incomplete": return PaySubscriptionStatus.Incomplete;

            case "incomplete_expired": return PaySubscriptionStatus.IncompleteExpired;
            case "trialing": return PaySubscriptionStatus.Trialing;
            case "active": return PaySubscriptionStatus.Active;
            case "past_due": return PaySubscriptionStatus.PastDue;
            case "canceled": return PaySubscriptionStatus.Cancelled;
            case "unpaid": return PaySubscriptionStatus.Unpaid;
            default:
                return PaySubscriptionStatus.None;
        }
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
}