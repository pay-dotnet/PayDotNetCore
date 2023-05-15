namespace PayDotNet.Stores;

public interface ICustomerStore : IModelStore<PayCustomer>
{
    IQueryable<PayCustomer> Customers { get; }
}