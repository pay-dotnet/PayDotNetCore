namespace PayDotNet.Core.Abstraction;

public interface ICustomerStore : IModelStore<PayCustomer>
{
    IQueryable<PayCustomer> Customers { get; }
}
