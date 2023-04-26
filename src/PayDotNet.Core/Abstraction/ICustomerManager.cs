using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ICustomerManager
{
    Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName);

    Task<PayCustomer?> FindByEmailAsync(string email, string processorName);

    Task<PayCustomer?> FindByIdAsync(string processorId, string processorName);

    Task UpdateAsync(PayCustomer customer);
}