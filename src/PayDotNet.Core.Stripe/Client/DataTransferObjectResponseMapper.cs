using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Stripe;

namespace PayDotNet.Core.Stripe.Client;

public class DataTransferObjectResponseMapper
{
    private readonly IOptions<PayDotNetConfiguration> _options;

    public DataTransferObjectResponseMapper(IOptions<PayDotNetConfiguration> options)
    {
        _options = options;
    }

    public PayChargeRefund Map(Refund refund)
    {
        return new()
        {
            ProcessorId = refund.Id,
            CreatedAt = refund.Created,
            Description = refund.Description,
            Amount = Convert.ToInt32(refund.Amount),
            Reason = refund.Reason,
            Status = refund.Status
        };
    }

    public PayCharge Map(Charge @object, Invoice? invoice)
    {
        ChargePaymentMethodDetails paymentMethodDetails = @object.PaymentMethodDetails;
        JToken? paymentMethod = paymentMethodDetails.RawJObject.GetValue(paymentMethodDetails.Type);

        PayCharge payCharge = new()
        {
            ProcessorId = @object.Id,

            Amount = Convert.ToInt32(@object.Amount),
            AmountCaptured = Convert.ToInt32(@object.AmountCaptured),
            AmountRefunded = Convert.ToInt32(@object.AmountRefunded),
            ApplicationFeeAmount = Convert.ToInt32(@object.ApplicationFeeAmount),
            Bank = GetBank(paymentMethod),
            Brand = paymentMethod?["brand"]?.Value<string>()?.ToUpper(),
            CreatedAt = @object.Created,
            Currency = @object.Currency,
            ExpirationMonth = paymentMethod?["exp_month"]?.Value<string>(),
            ExpirationYear = paymentMethod?["exp_year"]?.Value<string>(),
            Last4 = paymentMethod?["last4"]?.Value<string>(),
            LineItems = new List<PayChargeLineItem>(),
            PaymentIntentId = @object.PaymentIntentId,
            PaymentMethodType = @object.PaymentMethodDetails.Type,
            // TODO: StripeAccount = payCustomer.StripeAccount,
            ReceiptUrl = @object.ReceiptUrl,
            TotalTaxAmounts = new List<PayChargeTaxAmount>(),
            Refunds = @object.Refunds?.Select(Map).OrderBy(r => r.CreatedAt).ToArray() ?? Array.Empty<PayChargeRefund>(),

            Status = StripeStatusMapper.GetPayStatus(@object.Status),
        };

        invoice ??= @object.Invoice;
        if (invoice is not null)
        {
            payCharge.InvoiceId = invoice.Id;
            // TODO: payCharge.SubscriptionId = invoice.SubscriptionId; // TODO, find based on processorId

            payCharge.PeriodStart = invoice.PeriodStart;
            payCharge.PeriodEnd = invoice.PeriodEnd;
            payCharge.Subtotal = Convert.ToInt32(invoice.Subtotal);
            payCharge.Tax = Convert.ToInt32(invoice.Tax);

            payCharge.Discounts = invoice.DiscountIds;
            payCharge.TotalTaxAmounts = Map(invoice.TotalTaxAmounts);
            payCharge.TotalDiscountAmounts = Map(invoice.TotalDiscountAmounts);

            payCharge.LineItems = Map(invoice.Lines);
        }
        else
        {
            // Charges without invoices.
            payCharge.PeriodStart = @object.Created;
            payCharge.PeriodEnd = @object.Created;
        }

        return payCharge;
    }

    public PayPaymentMethod Map(PaymentMethod paymentMethod, bool isDefault = false)
    {
        return new PayPaymentMethod()
        {
            // External fields
            ProcessorId = paymentMethod.Id,
            IsDefault = isDefault,
            Type = paymentMethod.Type,
            CreatedAt = paymentMethod.Created,
            UpdatedAt = paymentMethod.Created,
        };
    }

    /// <summary>
    /// Maps the stripe subscription to a PaySubscription.
    /// </summary>
    /// <param name="object">The stripe subscription. </param>
    /// <returns>The pay subscription.</returns>
    public PaySubscription Map(Subscription @object)
    {
        PaySubscription paySubscription = new()
        {
            // External fields
            ProcessorId = @object.Id,
            ApplicationFeePercent = @object.ApplicationFeePercent,
            CreatedAt = @object.Created,
            ProcessorPlan = @object.Items.First().Price.Id,
            Quantity = Convert.ToInt32(@object.Items.First().Quantity),
            Status = StripeStatusMapper.GetSubscriptionStatus(@object.Status),
            // TODO: StripeAccount = payCustomer.StripeAccount.
            Name = @object.Metadata.TryOrDefault(PayMetadata.Fields.PaySubscriptionName, _options.Value.DefaultProductName),
            SubscriptionItems = new List<PaySubscriptionItem>(),
            IsMetered = false,
            PauseBehaviour = StripeStatusMapper.GetPauseBehaviour(@object.PauseCollection?.Behavior),
            PauseResumesAt = @object.PauseCollection?.ResumesAt,
            CurrentPeriodStart = @object.CurrentPeriodStart,
            CurrentPeriodEnd = @object.CurrentPeriodEnd,
        };

        // Subscriptions that have ended should have their trial ended at the same time if they were still on trial (if you cancel a
        // subscription, your are cancelling your trial as well at the same instant). This avoids Canceled subscriptions responding `true`
        // to IsOnTrial() due to the `trial_ends_at` being left set in the future.
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

            PaySubscriptionItem paySubscriptionItem = new()
            {
                Id = subscriptionItem.Id,
                Price = new()
                {
                    Id = subscriptionItem.Price.Id,
                },
                Quantity = Convert.ToInt32(subscriptionItem.Quantity),
            };
            paySubscription.SubscriptionItems.Add(paySubscriptionItem);
        }

        if (@object.EndedAt.HasValue)
        {
            // Fully cancelled paymentMethod
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

        Charge? charge = @object.LatestInvoice.Charge;
        if (charge is not null)
        {
            paySubscription.Charges.Add(Map(charge, @object.LatestInvoice));
        }

        return paySubscription;
    }

    private static string? GetBank(JToken? paymentMethod)
    {
        string? bank = paymentMethod?["bank_name"]?.Value<string>();
        if (string.IsNullOrEmpty(bank))
        {
            bank = paymentMethod?["bank"]?.Value<string>();
        }
        return bank;
    }

    /// <remarks>Implementation of: https://github.com/pay-rails/pay/blob/c7aa98f460a64d03c6fe51b2eb35763b0ee25315/lib/pay/receipts.rb#L71</remarks>
    private static string GetDiscountDescription(JObject discount)
    {
        JToken? coupon = discount?["discount"]?["coupon"];
        string? name = discount?["name"]?.Value<string>();
        string? percentOff = coupon?["percent_off"]?.Value<string>();

        // TODO: Resources for I18N
        if (!string.IsNullOrEmpty(percentOff))
        {
            return string.Format("{0} ({1:P2} off)", name, percentOff);
        }
        else
        {
            // Amount off
            return string.Format("{0} ({1} off)", name, percentOff);
        }
    }

    private static string GetTaxDescription(TaxRate taxRate)
    {
        return string.Format("{0} - {1} ({2:P} {3})",
            taxRate.DisplayName,
            taxRate.Jurisdiction,
            taxRate.Percentage,
            taxRate.Inclusive ? " inclusive" : string.Empty);
    }

    private static ICollection<PayChargeTotalDiscount> Map(List<InvoiceDiscountAmount>? discounts)
    {
        if (discounts is null)
        {
            return new List<PayChargeTotalDiscount>();
        }

        return discounts.Select(discount =>
        {
            return new PayChargeTotalDiscount()
            {
                DiscountId = discount.DiscountId,
                Amount = Convert.ToInt32(discount.Amount),
                Description = GetDiscountDescription(discount.RawJObject)
            };
        }).ToList();
    }

    private ICollection<PayChargeLineItem> Map(StripeList<InvoiceLineItem> lines)
    {
        return lines.Select(lineItem => new PayChargeLineItem
        {
            ProcessorId = lineItem.Id,
            Description = lineItem.Description,
            PriceId = lineItem.Price.Id,
            Quantity = Convert.ToInt32(lineItem.Quantity),
            UnitAmount = lineItem.Price.UnitAmount.HasValue ? Convert.ToInt32(lineItem.Price.UnitAmount.Value) : null,
            Amount = Convert.ToInt32(lineItem.Amount),
            Discounts = lineItem.DiscountIds,
            TaxAmounts = Map(lineItem.TaxAmounts),
            IsProration = lineItem.Proration,
            PeriodStart = lineItem.Period.Start,
            PeriodEnd = lineItem.Period.End,
        }).ToList();
    }

    private ICollection<PayChargeTaxAmount> Map(List<InvoiceTaxAmount> totalTaxAmounts)
    {
        return totalTaxAmounts.Select(invoiceTaxAmount => new PayChargeTaxAmount
        {
            Amount = Convert.ToInt32(invoiceTaxAmount.Amount),
            Description = GetTaxDescription(invoiceTaxAmount.TaxRate)
        }).ToList();
    }

    private ICollection<PayChargeTaxAmount> Map(List<InvoiceLineItemTaxAmount> lineTaxAmounts)
    {
        return lineTaxAmounts.Select(lineTaxAmount => new PayChargeTaxAmount
        {
            Amount = Convert.ToInt32(lineTaxAmount.Amount),
            Description = GetTaxDescription(lineTaxAmount.TaxRate)
        }).ToList();
    }
}