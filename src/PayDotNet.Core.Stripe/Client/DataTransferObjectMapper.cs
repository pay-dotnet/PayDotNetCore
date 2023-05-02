using System.Text;
using Microsoft.Extensions.Options;
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

    public PayCharge Map(Charge @object)
    {
        ChargePaymentMethodDetails paymentMethod = @object.PaymentMethodDetails;

        //string bank = string.Empty;
        //if (!paymentMethod.Metadata.TryGetValue("bank_name", out bank))
        //{
        //    paymentMethod.Metadata.TryGetValue("bank", out bank);
        //}

        PayCharge payCharge = new()
        {
            Amount = Convert.ToInt32(@object.Amount),
            AmountCaptured = Convert.ToInt32(@object.AmountCaptured),
            AmountRefunded = Convert.ToInt32(@object.AmountRefunded),
            ApplicationFeeAmount = Convert.ToInt32(@object.ApplicationFeeAmount),
            //Bank = bank
            Brand = paymentMethod.Card.Brand,
            CreatedAt = @object.Created,
            Currency = @object.Currency,
            ExpirationMonth = Convert.ToInt32(paymentMethod.Card.ExpMonth),
            ExpirationYear = Convert.ToInt32(paymentMethod.Card.ExpYear),
            Last4 = paymentMethod.Card.Last4,
            Metadata = @object.Metadata,
            PaymentIntentId = @object.PaymentIntentId,
            PaymentMethodType = @object.PaymentMethodDetails.Type,
            ReceiptUrl = @object.ReceiptUrl,
            Refunds = @object.Refunds.OrderBy(r => r.Created).Select(Map).ToArray()
        };

        if (@object.Invoice is not null)
        {
            Invoice invoice = @object.Invoice is Invoice
                ? @object.Invoice
                : default; // TODO: retrieve invoice

            payCharge.InvoiceId = invoice.Id;
            //payCharge.SubscriptionId = invoice.SubscriptionId; // TODO, find based on processorId
            payCharge.PeriodStart = invoice.PeriodStart;
            payCharge.PeriodEnd = invoice.PeriodEnd;
            payCharge.Subtotal = Convert.ToInt32(invoice.Subtotal);
            payCharge.Tax = Convert.ToInt32(invoice.Tax);
            //payCharge.Discounts = invoice.Discount;
            //payCharge.TotalTaxAmounts = invoice.TotalTaxAmounts;
            //payCharge.TotalDiscountAmounts = invoice.TotalDiscountAmounts;
            foreach (var lineItem in invoice.Lines)
            {
                payCharge.LineItems.Add(new()
                {
                    ProcessorId = lineItem.Id,
                    Description = lineItem.Description,
                    PriceId = lineItem.Price.Id,
                    Quantity = Convert.ToInt32(lineItem.Quantity),
                    UnitAmount = Convert.ToInt32(lineItem.Price.UnitAmount),
                    //Discounts = lineItem.Discounts,
                    //TaxAmounts = lineItem.TaxAmounts,
                    IsProration = lineItem.Proration,
                    PeriodStart = lineItem.Period.Start,
                    PeriodEnd = lineItem.Period.End,
                });
            }
        }
        else
        {
            payCharge.PeriodStart = @object.Created;
            payCharge.PeriodEnd = @object.Created;
        }

        return payCharge;
    }

    public PaySubscription Map(Subscription @object)
    {
        PaySubscription paySubscription = new()
        {
            Name = @object.Metadata.Try("pay_name") ?? _options.Value.DefaultProductName,
            ApplicationFeePercent = @object.ApplicationFeePercent,
            CreatedAt = @object.Created,
            ProcessorId = @object.Id,
            ProcessorPlan = @object.Items.First().Price.Id,
            Quantity = Convert.ToInt32(@object.Items.First().Quantity),
            Status = ConvertSubscriptionStatus(@object),
            IsMetered = false,
            PauseBehaviour = @object.PauseCollection?.Behavior,
            PauseResumesAt = @object.PauseCollection?.ResumesAt,
            CurrentPeriodStart = @object.CurrentPeriodStart,
            CurrentPeriodEnd = @object.CurrentPeriodEnd,
            Metadata = @object.Metadata,
        };

        if (@object.TrialEnd.HasValue)
        {
            @object.TrialEnd = new[] { @object.EndedAt, @object.TrialEnd }.Min();
        }

        // Record object items to model
        foreach (var subscriptionItem in @object.Items)
        {
            if (!paySubscription.IsMetered && subscriptionItem.Price.Recurring.UsageType == "metered")
            {
                paySubscription.IsMetered = true;
            }

            PaySubscriptionItem paySubscriptionItem = new()
            {
                Id = subscriptionItem.Id,
                Price = subscriptionItem.Price,
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