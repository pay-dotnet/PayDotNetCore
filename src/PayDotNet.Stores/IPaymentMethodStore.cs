namespace PayDotNet.Stores;

public interface IPaymentMethodStore : IModelStore<PayPaymentMethod>
{
    IQueryable<PayPaymentMethod> PaymentMethods { get; }
}

public interface IChargeStore : IModelStore<PayCharge>
{
    IQueryable<PayCharge> Charges { get; }
}