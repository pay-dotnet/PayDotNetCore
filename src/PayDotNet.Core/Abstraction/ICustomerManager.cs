using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ICustomerManager
{
    Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string processorId, string email);

    Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string email);

    Task<PayCustomer?> FindByEmailAsync(string processorName, string email);

    Task<PayCustomer?> FindByIdAsync(string processorName, string processorId);

    Task UpdateAsync(PayCustomer customer);
}