﻿namespace PayDotNet.Core.Models;

/// <summary>
/// TODO: required id+email
/// TODO: unique (processor, processorId)
/// </summary>
public class PayCustomer
{
    public string Id { get; set; }

    public string Email { get; set; }

    // TODO: make readonly so you can't add them
    public ICollection<PayCharges> Charges { get; init; } = new List<PayCharges>();

    // TODO: make readonly so you can't add them
    public ICollection<PaySubscription> Subscriptions { get; init; } = new List<PaySubscription>();

    // TODO: make readonly so you can't add them
    public ICollection<PayPaymentMethod> PaymentMethods { get; init; } = new List<PayPaymentMethod>();

    // Stripe, FakeProcessor, Braintree, etc.
    public string Processor { get; set; }

    public string? ProcessorId { get; set; }

    public bool IsDefault { get; set; }

    // TODO: JSON
    public string Data { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime DeletedAt { get; set; }

    public bool HasProcessorId()
    {
        return !string.IsNullOrEmpty(ProcessorId);
    }
}