namespace PayDotNet.Stores;

public interface IMerchantStore : IModelStore<PayMerchant>
{
    IQueryable<PayMerchant> Merchants { get; }
}