using PayDotNet.Core.Models;

namespace PayDotNet.Core.Services;

public abstract class PaymentProcessorSubscription : PaySubscription
{
    public abstract void Map(PaySubscription paySubscription);
};