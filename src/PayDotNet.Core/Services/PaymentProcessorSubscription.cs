using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public abstract class PaymentProcessorSubscription : PaySubscription
{
    public abstract void Map(PaySubscription paySubscription);
};