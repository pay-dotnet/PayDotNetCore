using Microsoft.Extensions.Options;
using PayDotNet.Core.Abstraction;
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

    private readonly ChargeService _charges;
    private readonly SessionService _checkoutSession;
    private readonly CreditNoteService _creditNotes;
    private readonly CustomerService _customers;
    private readonly InvoiceService _invoices;
    private readonly DataTransferObjectResponseMapper _mapper;
    private readonly IOptions<PayDotNetConfiguration> _options;
    private readonly PaymentIntentService _paymentIntents;
    private readonly PaymentMethodService _paymentMethods;
    private readonly RefundService _refunds;
    private readonly SetupIntentService _setupIntents;
    private readonly IStripeClient _stripeClient;
    private readonly SubscriptionService _subscriptions;

    public StripePaymentProcessorService(
        IStripeClient stripeClient,
        IOptions<PayDotNetConfiguration> options)
    {
        _stripeClient = stripeClient;
        _options = options;
        _mapper = new(options);

        _charges = new(_stripeClient);
        _customers = new(_stripeClient);
        _invoices = new(_stripeClient);
        _subscriptions = new(_stripeClient);
        _paymentMethods = new(_stripeClient);
        _paymentIntents = new(_stripeClient);
        _setupIntents = new(_stripeClient);
        _refunds = new(_stripeClient);
        _checkoutSession = new(_stripeClient);
        _creditNotes = new(_stripeClient);
    }

    public string Name => PaymentProcessors.Stripe;

    public bool IsPaymentMethodRequired(PayCustomer payCustomer)
    {
        return false;
    }

    private static void GuardInvoiceId(PayCharge payCharge)
    {
        if (string.IsNullOrEmpty(payCharge.InvoiceId))
        {
            throw new PayDotNetStripeException("No InvoiceId on PayCharge");
        }
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

    private async Task TryAsync(Func<Task> value)
    {
        try
        {
            await value.Invoke().WaitAsync(CancellationToken.None);
        }
        catch (StripeException exception)
        {
            throw new PayDotNetStripeException(PayDotNetStripeException.DefaultMessage, exception);
        }
    }

    internal static class Expand
    {
        public static readonly List<string> Charge = new()
        {
            "invoice.total_discount_amounts.discount",
            "invoice.total_tax_amounts.tax_rate",
            "refunds"
        };

        public static readonly List<string> Checkout = new()
        {
            "payment_intent",
            "setup_intent"
        };

        public static readonly List<string> Customer = new() { "tax" };

        public static readonly List<string> Invoice = new()
        {
            "total_discount_amounts.discount",
            "total_tax_amounts.tax_rate"
        };

        public static readonly List<string> PaymentIntent = new()
        {
            "latest_charge",
            "latest_charge.refunds"
        };

        public static readonly List<string> Subscription = new()
        {
            "pending_setup_intent",
            "latest_invoice",
            "latest_invoice.payment_intent",
            "latest_invoice.charge",
            "latest_invoice.charge.refunds"
        };
    }

    #region Customer API

    public Task<string> CreateCustomerAsync(PayCustomer payCustomer)
    {
        return TryAsync(async () =>
        {
            Customer customer = await _customers.CreateAsync(GetCustomerCreateOptions(payCustomer));
            return customer.Id;
        });
    }

    /// <remarks>
    /// TODO: find a smart way to allow pay customers to synchronise other attribute to the payment processors.
    /// </remarks>
    public virtual CustomerCreateOptions GetCustomerCreateOptions(PayCustomer payCustomer)
    {
        return new CustomerCreateOptions
        {
            Email = payCustomer.Email,
            Expand = Expand.Customer,
        };
    }

    /// <remarks>
    /// TODO: find a smart way to allow pay customers to synchronise other attribute to the payment processors.
    /// </remarks>
    public virtual CustomerUpdateOptions GetCustomerUpdateOptions(PayCustomer payCustomer)
    {
        return new()
        {
            Email = payCustomer.Email,
            Expand = Expand.Customer,
        };
    }

    public Task UpdateCustomerAsync(PayCustomer payCustomer)
    {
        return TryAsync(async () =>
        {
            Customer _ = await _customers.UpdateAsync(payCustomer.Processor, GetCustomerUpdateOptions(payCustomer));
        });
    }

    #endregion Customer API

    #region Payment method API

    /// <inheritdoc/>
    public Task<PayPaymentMethod> AttachPaymentMethodAsync(PayCustomer payCustomer, PayPaymentMethodOptions options)
    {
        return TryAsync(async () =>
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

            return _mapper.Map(paymentMethod, options.IsDefault);
        });
    }

    public Task<PayPaymentMethod?> GetPaymentMethodAsync(PayCustomer payCustomer, string processorId)
    {
        return TryAsync(async () =>
        {
            PaymentMethod? paymentMethod = await _paymentMethods.GetAsync(processorId);
            if (paymentMethod is null)
            {
                return null;
            }

            return _mapper.Map(paymentMethod);
        });
    }

    #endregion Payment method API

    #region Subscriptions API

    public Task CancelAsync(PayCustomer payCustomer, PaySubscription paySubscription, PayCancelSubscriptionOptions options)
    {
        return TryAsync(async () =>
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
        });
    }

    /// <remarks>
    /// If customer has no default payment method, we MUST allow the subscription to be incomplete.
    /// Then the caller, can decide if they want to redirect to the payment form.
    /// </remarks>
    public Task<PaySubscriptionResult> CreateSubscriptionAsync(PayCustomer payCustomer, PaySubscribeOptions options)
    {
        return TryAsync(async () =>
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
                    [PayMetadata.Fields.PaySubscriptionName] = string.IsNullOrEmpty(options.Name) ? _options.Value.DefaultPlanName : options.Name
                },
                Expand = Expand.Subscription,
            };

            Subscription subscription = await _subscriptions.CreateAsync(stripeOptions);

            PaySubscription paySubscription = _mapper.Map(subscription);
            IPayment? payment = new StripePaymentIntentPayment(subscription.LatestInvoice.PaymentIntent);
            return new PaySubscriptionResult(paySubscription, payment);
        });
    }

    public Task<PaySubscriptionResult?> GetSubscriptionAsync(PayCustomer payCustomer, string processorId)
    {
        return TryAsync(async () =>
        {
            SubscriptionGetOptions stripeOptions = new()
            {
                Expand = Expand.Subscription,
            };

            Subscription? subscription = await _subscriptions.GetAsync(processorId, stripeOptions);
            if (subscription is null)
            {
                return null;
            }

            PaySubscription paySubscription = _mapper.Map(subscription);
            IPayment? payment = new StripePaymentIntentPayment(subscription.LatestInvoice.PaymentIntent);
            return new PaySubscriptionResult(paySubscription, payment);
        });
    }

    #endregion Subscriptions API

    #region Charges API

    /// <inheritdoc/>
    /// <remarks>
    /// For more information, see: https://stripe.com/docs/payments/place-a-hold-on-a-payment-method
    /// </remarks>
    public Task<IPayment> CaptureAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeCaptureOptions options)
    {
        return TryAsync<IPayment>(async () =>
        {
            if (string.IsNullOrEmpty(payCharge.PaymentIntentId))
            {
                throw new PayDotNetStripeException("The pay charge is missing the PaymentIntentId");
            }

            if (payCharge.Status != PayStatus.RequiresCapture)
            {
                throw new PayDotNetStripeException("The pay charge is not in an \"to capture\" state. Unable to capture this pay charge.");
            }

            PaymentIntent paymentIntent = await _paymentIntents.CaptureAsync(payCharge.ProcessorId, new()
            {
                AmountToCapture = options.AmountToCapture
            });

            return new StripePaymentIntentPayment(paymentIntent);
        });
    }

    /// <inheritdoc/>
    /// <remarks>
    /// If the <paramref name="options"/> contain a PaymentMethodId, we assume it has been set to the default for the customer in the billable manager.
    /// <br/>
    /// If the <paramref name="options"/> does not contain a PaymentMethodId, we use <see cref="PayCustomer.DefaultPaymentMethod"/>.
    /// <br/>
    /// Finally, if neither of these cases are valid, Stripe will comeback with status "payment_method_required".
    /// The frontend/caller can handle this flow themselves
    /// </remarks>
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
                Expand = Expand.PaymentIntent,

                PaymentMethod = string.IsNullOrEmpty(options.PaymentMethodId)
                    ? payCustomer.DefaultPaymentMethod?.ProcessorId
                    : options.PaymentMethodId,
            });

            return new PayChargeResult(
                paymentIntent.LatestCharge is null ? null : _mapper.Map(paymentIntent.LatestCharge, invoice: null),
                new StripePaymentIntentPayment(paymentIntent));
        });
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns null if the customer is not specified on the charge.
    /// </remarks>
    public Task<PayCharge?> GetChargeAsync(PayCustomer payCustomer, string processorId)
    {
        return TryAsync(async () =>
        {
            Charge? charge = await _charges.GetAsync(processorId, new()
            {
                Expand = Expand.Charge
            });

            if (charge is null || charge.Customer is null || charge.CustomerId != payCustomer.ProcessorId)
            {
                return null;
            }

            if (charge.Invoice is null && string.IsNullOrEmpty(charge.InvoiceId))
            {
                return _mapper.Map(charge, null);
            }

            Invoice? invoice = charge.Invoice;
            invoice ??= await GetInvoiceAsync(charge.InvoiceId);
            return _mapper.Map(charge, invoice: invoice);
        });
    }

    public Task<IPayment> GetPaymentAsync(PayCustomer payCustomer, string processorId)
    {
        return TryAsync<IPayment>(async () =>
        {
            // TODO: check if this charge is actually for this customer.
            return processorId.StartsWith("seti_")
                ? new StripeSetupIntentPayment(await _setupIntents.GetAsync(processorId))
                : new StripePaymentIntentPayment(await _paymentIntents.GetAsync(processorId));
        });
    }

    private Task<Invoice?> GetInvoiceAsync(string processorId)
    {
        return TryAsync<Invoice?>(() => _invoices.GetAsync(processorId, new()
        {
            Expand = Expand.Invoice
        }));
    }

    #endregion Charges API

    #region Checkout API

    /// <inheritdoc/>
    public Task<PayCheckoutResult> CheckoutAsync(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        return TryAsync(async () =>
        {
            Session session = await _checkoutSession.CreateAsync(GetCheckoutOptions(payCustomer, options));
            return new PayCheckoutResult(
                session.Id,
                new Uri(session.Url),
                new Uri(session.SuccessUrl),
                session.Mode);
        });
    }

    private SessionCreateOptions GetCheckoutOptions(PayCustomer payCustomer, PayCheckoutOptions options)
    {
        string successUrl = string.IsNullOrEmpty(options.SuccessUrl) ? _options.Value.RootUrl : options.SuccessUrl;
        string cancelUrl = string.IsNullOrEmpty(options.CancelUrl) ? _options.Value.RootUrl : options.CancelUrl;

        bool isSetup = options.Mode == "setup";

        return new()
        {
            Expand = Expand.Checkout,

            Mode = options.Mode,
            Customer = payCustomer.ProcessorId,
            ClientReferenceId = options.ClientReferenceId,

            // Setup specific required fields.
            AllowPromotionCodes = isSetup ? null : options.AllowPromotionCodes,
            PaymentMethodTypes = isSetup ? _options.Value.Stripe.PaymentMethodTypes : null,

            SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{cancelUrl}",

            // Line items are optional.
            LineItems = options.LineItems.None()
                ? null
                : options.LineItems.Select(li =>
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
        };
    }

    #endregion Checkout API

    #region Refunds API

    public Task IssueCreditNotesAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        GuardInvoiceId(payCharge);
        return TryAsync(async () =>
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
        });
    }

    public Task<PayChargeRefund> RefundAsync(PayCustomer payCustomer, PayCharge payCharge, PayChargeRefundOptions options)
    {
        return TryAsync(async () =>
        {
            Refund refund = await _refunds.CreateAsync(new()
            {
                Charge = payCharge.ProcessorId,
                Amount = options.Amount,
            });
            return _mapper.Map(refund);
        });
    }

    #endregion Refunds API
}