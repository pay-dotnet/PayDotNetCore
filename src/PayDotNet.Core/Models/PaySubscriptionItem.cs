namespace PayDotNet.Core.Models;

public class PaySubscriptionItem
{
    public string Id { get; set; } = default!;

    public PaySubscriptionItemPrice Price { get; set; } = default!;

    public Dictionary<string, string> Metadata { get; set; } = new();

    public long Quantity { get; set; }
}

public class PaySubscriptionItemPrice
{
    public string Id { get; set; }
}