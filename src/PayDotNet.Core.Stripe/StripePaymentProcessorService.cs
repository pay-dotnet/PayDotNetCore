using Microsoft.Extensions.Options;
using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Stripe.Client;
using Stripe;
using Stripe.Checkout;

namespace PayDotNet.Core.Stripe;

public class StripePaymentProcessorService : IPaymentProcessorService
{
    public static readonly AppInfo AppInfo = new AppInfo()
    {
        Name = "Pay.DotNet",
        PartnerId = "TODO",
        Url = "https://github.com/pay-dotnet/PayDotNetCore"
    };

    private readonly PaymentIntentService _paymentIntents;
    private readonly ChargeService _charges;
    private readonly CreditNoteService _creditNotes;
    private readonly RefundService _refunds;
    private readonly SessionService _checkoutSession;
    private readonly CustomerService _customers;
    private readonly SubscriptionService _subscriptions;
    private readonly PaymentMethodService _paymentMethods;
    private readonly SetupIntentService _setupIntents;
    private readonly IStripeClient _stripeClient;
    private readonly IOptions<PayDotNetConfiguration> _options;
    private readonly DataTransferObjectMapper _mapper;

    public bool IsPaymentMethodRequired(PayCustomer payCustomer)
    {
        return false;
    }

    public string Name => PaymentProcessors.Stripe;

    public StripePaymentProcessorService(
        IStripeClient stripeClient,
        IOptions<PayDotNetConfiguration> options)
    {
        _stripeClient = stripeClient;
        _options = options;
        _mapper = new(options);

        _charges = new(_stripeClient);
        _customers = new(_stripeClient);
        _subscriptions = new(_stripeClient);
        _paymentMethods = new(_stripeClient);
        _paymentIntents = new(_stripeClient);
        _setupIntents = new(_stripeClient);
        _refunds = new(_stripeClient);
        _checkoutSession = new(_stripeClient);
        _creditNotes = new(_stripeClient);
    }

    private static readonly List<string> DefaultSubscriptionExpandOptions = new()
    {
        "pending_setup_intent",
        "latest_invoice",
        "latest_invoice.payment_intent",
        "latest_invoice.charge",
        "latest_invoice.charge.refunds"
    };

    #region Customer API

    public async Task<string> CreateCustomerAsync(PayCustomer payCustomer)
    {
        Customer customer = await _customers.CreateAsync(new CustomerCreateOptions
        {
            // TODO: customer attributes
            Email = payCustomer.Email,
            Expand = new() { "tax" },
        });

        return customer.Id;
    }

    #endregion Customer API

    #region Payment method API

    public async Task<PayPaymentMethod> AttachPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options)
    {
        PaymentMethod paymentMethod = await _paymentMethods.AttachAsync(options.PaymentMethodId, new()
        {
            Customer = payCustomer.ProcessorId,
        });

        if (options.IsDefault)
        {
            await _customers.UpdateAsync(payCustomer.ProcessorId, new CustomerUpdateOptions
            {
                InvoiceSettings = new()
                {
                    DefaultPaymentMethod = paymentMethod.Id
                }
            });
        }

        return new PayPaymentMethod()
        {
            // Internal fields
            CustomerId = payCustomer.Id,

            // External fields
            ProcessorId = paymentMethod.Id,
            IsDefault = options.IsDefault,
            Type = paymentMethod.Type,
            CreatedAt = paymentMethod.Created,
            UpdatedAt = paymentMethod.Created,
        };
    }

    #endregion Payment method API

    #region Subscriptions API

    /// <remarks>
    /// If customer has no default payment method, we MUST allow the subscription to be incomplete.
    /// Then the caller, can decide if they want to redirect to the payment form.
    /// </remarks>
    public async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        string paymentBehaviour = payCustomer.DefaultPaymentMethod is null
            ? "default_incomplete"
            : _options.Value.Stripe.PaymentBehaviour;

        SubscriptionCreateOptions stripeOptions = new SubscriptionCreateOptions
        {
            Customer = payCustomer.ProcessorId,
            Items = options.Items.Select(p => new SubscriptionItemOptions()
            {
                Price = p.PriceId,
                Quantity = p.Quantity
            }).ToList(),
            PaymentBehavior = paymentBehaviour,
            Metadata = new()
            {
                ["pay_name"] = string.IsNullOrEmpty(options.Name) ? _options.Value.DefaultPlanName : options.Name
            },
            Expand = DefaultSubscriptionExpandOptions,
        };

        Subscription subscription = await _subscriptions.CreateAsync(stripeOptions);

        PaySubscription paySubscription = _mapper.Map(subscription);
        IPayment? payment = new StripePaymentIntentPayment(subscription.LatestInvoice.PaymentIntent);
        return new(paySubscription, payment);
    }

    public async Task<PaySubscriptionResult?> GetSubscriptionAsync(PayCustomer payCustomer, string processorId)
    {
        SubscriptionGetOptions stripeOptions = new()
        {
            Expand = DefaultSubscriptionExpandOptions,
        };

        Subscription? subscription = await _subscriptions.GetAsync(processorId, stripeOptions);
        if (subscription is null)
        {
            return null;
        }

        PaySubscription paySubscription = _mapper.Map(subscription);
        IPayment? payment = new StripePaymentIntentPayment(subscription.LatestInvoice.PaymentIntent);
        return new(paySubscription, payment);
    }

    public async Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options)
    {
        await _subscriptions.CancelAsync(paySubscription.ProcessorId, new()
        {
            Prorate = options.Prorate,
            CancellationDetails = new()
            {
                Comment = options.Comment,
                Feedback = options.Feedback.ToString().ToSnakeCase(),
            }
        });
    }

    #endregion Subscriptions API

    #region Charges API

    public async Task<PayCharge> GetChargeAsync(PayCustomer payCustomer, string processorId)
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
        return _mapper.Map(charge, invoice: null);
    }

    public async Task<IPayment> GetPaymentAsync(PayCustomer payCustomer, string processorId)
    {
        // TODO: check if this charge is actually for this customer.
        return processorId.StartsWith("seti_")
            ? new StripeSetupIntentPayment(await _setupIntents.GetAsync(processorId))
            : new StripePaymentIntentPayment(await _paymentIntents.GetAsync(processorId));
    }

    public async Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeOptions options)
    {
        PaymentIntent paymentIntent = await _paymentIntents.CaptureAsync(payCharge.ProccesorId, new()
        {
            //TODO: AmountToCapture = stripeOptions.AmountToCapture
        });

        return new StripePaymentIntentPayment(paymentIntent);
    }

    public Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        return TryAsync(async () =>
        {
            PaymentIntent paymentIntent = await _paymentIntents.CreateAsync(new()
            {
                Customer = payCustomer.ProcessorId,
                Confirm = true,
                Amount = options.Amount,
                Currency = options.Currency,
                CaptureMethod = options.CaptureMethod,
                Expand = new() { "latest_charge", "latest_charge.refunds" },
                PaymentMethod = string.IsNullOrEmpty(options.PaymentMethodId)
                    ? payCustomer.DefaultPaymentMethod?.ProcessorId
                    : options.PaymentMethodId,
            });

            return new PayChargeResult(
                paymentIntent.LatestCharge is null ? null : _mapper.Map(paymentIntent.LatestCharge, invoice: null),
                new StripePaymentIntentPayment(paymentIntent));
        });
    }

    private async Task<TResult> TryAsync<TResult>(Func<Task<TResult>> value)
    {
        try
        {
            return await value.Invoke().WaitAsync(CancellationToken.None);
        }
        catch (StripeException exception)
        {
            throw new PayDotNetStripeException(PayDotNetStripeException.DefaultMessage, exception);
        }
    }

    #endregion Charges API

    #region Checkout API

    public async Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        string successUrl = string.IsNullOrEmpty(options.SuccessUrl) ? _options.Value.RootUrl : options.SuccessUrl;
        string cancelUrl = string.IsNullOrEmpty(options.CancelUrl) ? _options.Value.RootUrl : options.CancelUrl;

        bool isSetup = options.Mode == "setup";
        Session session = await _checkoutSession.CreateAsync(new()
        {
            Mode = options.Mode,
            Customer = payCustomer.ProcessorId,
            ClientReferenceId = options.ClientReferenceId,

            // Setup specific required fields.
            AllowPromotionCodes = isSetup ? null : options.AllowPromotionCodes,
            PaymentMethodTypes = isSetup ? _options.Value.Stripe.PaymentMethodTypes : null,

            SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{cancelUrl}",

            // What are we checking out?
            // TODO: move to mapper or seperate method.
            LineItems = options.LineItems.None() ? null : options.LineItems.Select(li =>
            {
                if (li.PriceData is null)
                {
                    return new SessionLineItemOptions()
                    {
                        Quantity = li.Quantity,
                        Price = li.PriceId,
                    };
                }
                else
                {
                    return new SessionLineItemOptions()
                    {
                        Quantity = li.Quantity,
                        PriceData = new()
                        {
                            Currency = li.PriceData.Currency,
                            UnitAmount = li.PriceData.UnitAmount,
                            ProductData = new()
                            {
                                Description = li.PriceData.Description,
                                Images = li.PriceData.Images,
                                Name = li.PriceData.Name
                            },
                        },
                    };
                }
            }).ToList(),

            Expand = new() { "payment_intent", "setup_intent" },
        });

        return new PayCheckoutResult(
            session.Id,
            new Uri(session.Url),
            new Uri(session.SuccessUrl),
            session.Mode);
    }

    #endregion Checkout API

    #region Refunds API

    public async Task<PayChargeRefund> RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        try
        {
            Refund refund = await _refunds.CreateAsync(new()
            {
                Charge = payCharge.ProccesorId,
                Amount = options.Amount,
            });
            return _mapper.Map(refund);
        }
        catch (StripeException exception)
        {
            throw new PayDotNetStripeException(PayDotNetStripeException.DefaultMessage, exception);
        }
    }

    public async Task IssueCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        GuardInvoiceId(payCharge);
        try
        {
            CreditNoteService creditNoteService = new();
            CreditNote creditNote = await creditNoteService.CreateAsync(new()
            {
                Invoice = payCharge.InvoiceId,
                RefundAmount = options.Amount,
                Lines = new()
                {
                    new CreditNoteLineOptions()
                    {
                        Type = "custom_line_item",
                        Description = string.IsNullOrEmpty(options.Description) ? _options.Value.DefaultRefundDescription : options.Description,
                        UnitAmount = options.Amount,
                        Quantity = 1,
                    }
                },
            });
        }
        catch (StripeException exception)
        {
            throw new PayDotNetStripeException(PayDotNetStripeException.DefaultMessage, exception);
        }
    }

    public async Task<ICollection<object>> GetCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge)
    {
        GuardInvoiceId(payCharge);
        StripeList<CreditNote> creditNotes = await _creditNotes.ListAsync(new()
        {
            Invoice = payCharge.InvoiceId,
            Customer = payCustomer.ProcessorId
        });
        return new List<object> { creditNotes };
    }

    #endregion Refunds API

    private static void GuardInvoiceId(PayCharge payCharge)
    {
        if (string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            throw new PayDotNetStripeException("No InvoiceId on PayCharge");
        }
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