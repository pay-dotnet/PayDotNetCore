using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface IPaymentMethodStore : IModelStore<PayPaymentMethod>
{
    IQueryable<PayPaymentMethod> PaymentMethods { get; }
}