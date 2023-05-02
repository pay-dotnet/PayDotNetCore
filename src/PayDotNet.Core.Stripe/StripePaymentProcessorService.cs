using Microsoft.Extensions.Options;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;
using PayDotNet.Core.Stripe.Client;
using Stripe;
using Stripe.Checkout;

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

    #region Subscriptions API

    public async Task<PayPaymentMethod> AttachPaymentMethodAsync(PayCustomer payCustomer, string paymentMethodId, bool isDefault)
    {
        PaymentMethod paymentMethod = await _paymentMethods.AttachAsync(paymentMethodId, new()
        {
            Customer = payCustomer.ProcessorId,
        });

        if (isDefault)
        {
            await _customers.UpdateAsync(payCustomer.ProcessorId, new CustomerUpdateOptions
            {
                InvoiceSettings = new()
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            });
        }

        return new PayPaymentMethod()
        {
            // Internal fields
            CustomerId = payCustomer.Id,

            // External fields
            ProcessorId = paymentMethod.Id,
            IsDefault = isDefault,
            Type = paymentMethod.Type,
            CreatedAt = paymentMethod.Created,
            UpdatedAt = paymentMethod.Created,
        };
    }

    #endregion Subscriptions API

    public async Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        // If customer has no default payment method, we MUST allow the subscription to be incomplete.
        // Then the caller, can decide if they want to redirect to the payment form.
        string paymentBehaviour = !payCustomer.PaymentMethods.Any(p => p.IsDefault)
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
        };

        stripeOptions.AddExpand("pending_setup_intent");
        stripeOptions.AddExpand("latest_invoice");
        stripeOptions.AddExpand("latest_invoice.payment_intent");
        stripeOptions.AddExpand("latest_invoice.charge");

        Subscription subscription = await _subscriptions.CreateAsync(stripeOptions);

        PaySubscription paySubscription = _mapper.Map(subscription);
        IPayment? payment = new StripePaymentIntentPayment(subscription.LatestInvoice.PaymentIntent);
        return new(paySubscription, payment);
    }

    public async Task<PaySubscriptionResult?> GetSubscriptionAsync(string processorId)
    {
        Subscription? subscription = await _subscriptions.GetAsync(processorId, new()
        {
            Expand = DefaultSubscriptionExpandOptions,
        });
        if (subscription is null)
        {
            return null;
        }

        PaySubscription paySubscription = _mapper.Map(subscription);
        IPayment? payment = new StripePaymentIntentPayment(subscription.LatestInvoice.PaymentIntent);
        return new(paySubscription, payment);
    }

    public async Task CancelAsync(PaySubscription paySubscription, PayCancelSubscriptionOptions options)
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

    #endregion Payment method API

    #region Charges API

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
        return processorId.StartsWith("seti_")
            ? new StripeSetupIntentPayment(await _setupIntents.GetAsync(processorId))
            : new StripePaymentIntentPayment(await _paymentIntents.GetAsync(processorId));
    }

    public async Task<IPayment> CaptureAsync(PayCharge payCharge, PayChargeOptions options)
    {
        PaymentIntent paymentIntent = await _paymentIntents.CaptureAsync(payCharge.ProccesorId, new()
        {
            //TODO: AmountToCapture = options.AmountToCapture
        });

        return new StripePaymentIntentPayment(paymentIntent);
    }

    public async Task<PayChargeResult> ChargeAsync(PayCustomer payCustomer, PayChargeOptions options)
    {
        PaymentIntent paymentIntent = await _paymentIntents.CreateAsync(new()
        {
            Customer = payCustomer.ProcessorId,
            Confirm = true,
            Amount = options.Amount,
            Currency = options.Currency,
            CaptureMethod = options.CaptureMethod,
            Expand = new() { "latest_charge.refunds" }
        });

        return new PayChargeResult(
            _mapper.Map(paymentIntent.LatestCharge),
            new StripePaymentIntentPayment(paymentIntent));
    }

    #endregion Charges API

    #region Checkout API

    public async Task<Uri> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        string successUrl = string.IsNullOrEmpty(options.SuccessUrl) ? _options.Value.RootUrl : options.SuccessUrl;
        string cancelUrl = string.IsNullOrEmpty(options.CancelUrl) ? _options.Value.RootUrl : options.CancelUrl;

        var services = new SessionService();
        Session session = await services.CreateAsync(new()
        {
            AllowPromotionCodes = options.AllowPromotionCodes,
            Customer = payCustomer.ProcessorId,
            Mode = options.Mode,
            SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{cancelUrl}",
            LineItems = options.LineItems.Select(li =>
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
            }).ToList()
        });
        return new Uri(session.Url);
    }

    #endregion Checkout API

    #region Refunds API

    public async Task RefundAsync(PayCharge payCharge, PayChargeRefundOptions options)
    {
        try
        {
            RefundService refundService = new();
            await refundService.CreateAsync(new()
            {
                Charge = payCharge.ProccesorId,
                Amount = options.Amount,
            });
        }
        catch (StripeException exception)
        {
            throw new PayDotNetStripeException(PayDotNetStripeException.DefaultMessage, exception);
        }
    }

    public async Task IssueCreditNotesAsync(PayCharge payCharge, PayChargeRefundOptions options)
    {
        GuardInvoiceId(payCharge);
        try
        {
            CreditNoteService creditNoteService = new();
            await creditNoteService.CreateAsync(new()
            {
                RefundAmount = options.Amount,
                Lines = new()
                {
                    new CreditNoteLineOptions()
                    {
                        Description = string.IsNullOrEmpty(options.Description) ? _options.Value.DefaultRefundDescription : options.Description,
                        Amount = options.Amount,
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
        CreditNoteService creditNoteService = new();
        StripeList<CreditNote> creditNotes = await creditNoteService.ListAsync(new()
        {
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