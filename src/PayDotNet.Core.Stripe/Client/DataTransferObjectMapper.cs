using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PayDotNet.Core.Models;
using Stripe;

namespace PayDotNet.Core.Stripe.Client;

public class DataTransferObjectMapper
{
    private readonly IOptions<PayDotNetConfiguration> _options;

    public DataTransferObjectMapper(IOptions<PayDotNetConfiguration> options)
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

    /// <summary>
    /// Maps the charge and invoice to a PayCharge that can be stored.
    /// </summary>
    /// <param name="object">The stripe charge.</param>
    /// <param name="invoice">The associated stripe invoice if exist.</param>
    /// <returns>The pay charge.</returns>
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
            Metadata = @object.Metadata,
            PaymentIntentId = @object.PaymentIntentId,
            PaymentMethodType = @object.PaymentMethodDetails.Type,
            // TODO: StripeAccount = payCustomer.StripeAccount,
            ReceiptUrl = @object.ReceiptUrl,
            TotalTaxAmounts = new List<PayChargeTotalTaxAmount>(),
            Refunds = @object.Refunds?.Select(Map).OrderBy(r => r.CreatedAt).ToArray() ?? Array.Empty<PayChargeRefund>()
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
            //TODO: payCharge.TotalTaxAmounts = Map(invoice.TotalTaxAmounts);
            //TODO: payCharge.TotalDiscountAmounts = Map(invoice.TotalDiscountAmounts);

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

    // https://github.com/pay-rails/pay/blob/master/lib/pay/receipts.rb#L41
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
            //TODO: TaxAmounts = lineItem.TaxAmounts,
            IsProration = lineItem.Proration,
            PeriodStart = lineItem.Period.Start,
            PeriodEnd = lineItem.Period.End,
        }).ToList();
    }

    private ICollection<PayChargeTotalTaxAmount> Map(List<InvoiceTaxAmount> totalTaxAmounts)
    {
        return totalTaxAmounts.Select(invoiceTaxAmount => new PayChargeTotalTaxAmount
        {
            Amount = Convert.ToInt32(invoiceTaxAmount.Amount),
            //Description = GetDiscountDescription(invoiceTaxAmount.RawJObject)
        }).ToList();
    }

    /// <remarks>Implementation of: https://github.com/pay-rails/pay/blob/c7aa98f460a64d03c6fe51b2eb35763b0ee25315/lib/pay/receipts.rb#L71</remarks>
    private static string? GetDiscountDescription(JObject discount)
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
                // TODO;
            };
        }).ToList();
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
            Status = ConvertSubscriptionStatus(@object),
            // TODO: StripeAccount = payCustomer.StripeAccount.
            Metadata = @object.Metadata,
            SubscriptionItems = new List<PaySubscriptionItem>(),
            IsMetered = false,
            PauseBehaviour = @object.PauseCollection?.Behavior,
            PauseResumesAt = @object.PauseCollection?.ResumesAt,
            CurrentPeriodStart = @object.CurrentPeriodStart,
            CurrentPeriodEnd = @object.CurrentPeriodEnd,
        };

        // Subscriptions that have ended should have their trial ended at the same time if they were still on trial (if you cancel a
        // subscription, your are cancelling your trial as well at the same instant). This avoids canceled subscriptions responding `true`
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
                Metadata = subscriptionItem.Metadata,
                Quantity = subscriptionItem.Quantity
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

    private static PaySubscriptionStatus ConvertSubscriptionStatus(Subscription subscription)
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

    internal PayPaymentMethod Map(PayCustomer payCustomer, PaymentMethod paymentMethod, bool isDefault = false)
    {
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
}

internal static class Extensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder result = new StringBuilder();
        bool isFirst = true;

        foreach (char c in input)
        {
            if (Char.IsWhiteSpace(c))
            {
                continue;
            }

            if (Char.IsUpper(c))
            {
                if (!isFirst)
                {
                    result.Append("_");
                }

                result.Append(Char.ToLower(c));
            }
            else
            {
                result.Append(c);
            }

            isFirst = false;
        }

        return result.ToString();
    }
}