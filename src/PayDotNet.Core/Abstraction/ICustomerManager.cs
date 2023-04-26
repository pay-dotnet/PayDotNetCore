using PayDotNet.Core.Models;

namespace PayDotNet.Core.Abstraction;

public interface ICustomerManager
{
    // TODO: reorder params
    Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string processorId, string email);

    Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName);

    Task<PayCustomer?> FindByEmailAsync(string email, string processorName);

    Task<PayCustomer?> FindByIdAsync(string processorName, string processorId);

    Task UpdateAsync(PayCustomer customer);
}