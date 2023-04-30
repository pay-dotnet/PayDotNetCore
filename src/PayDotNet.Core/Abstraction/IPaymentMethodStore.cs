using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentMethodStore : IModelStore<PayPaymentMethod>
{
    IQueryable<PayPaymentMethod> PaymentMethods { get; }
}

public interface IChargeStore : IModelStore<PayCharge>
{
    IQueryable<PayCharge> Charges { get; }
}