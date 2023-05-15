namespace PayDotNet.Core;

public record PaySubscribeOptions(string Name, List<PaySubscribeOptionsItem> Items)
{
    public PaySubscribeOptions(string Name, string PriceId)
        : this(Name, new List<PaySubscribeOptionsItem>() { new(PriceId) })
    {
    }
}

public record PayCustomerOptions(string ProcessorName, string? PaymentMethodId = null, bool AllowFake = false);

public record PayCancelSubscriptionOptions(CancellationReason Feedback = CancellationReason.Other, string? Comment = null, bool Prorate = false, bool CancelAtEndPeriod = true);