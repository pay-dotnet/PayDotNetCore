﻿namespace PayDotNet.Core.Models;

/// TODO: unique(customer, processorId)
public class PayPaymentMethod : Timestamps
{
    public string CustomerId { get; set; }

    public bool IsDefault { get; set; }

    public string ProcessorId { get; set; }

    public string Type { get; set; }
}