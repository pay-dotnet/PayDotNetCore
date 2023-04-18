using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Managers;

public class CustomerManager : ICustomerManager
{
    private readonly ICustomerStore _customerStore;

    public CustomerManager(ICustomerStore customerStore)
    {
        _customerStore = customerStore;
    }

    public async Task<PayCustomer> GetOrCreateCustomerAsync(string email, string processorName)
    {
        PayCustomer? customer = _customerStore.Customers.FirstOrDefault(c => c.Email == email && c.Processor == processorName);
        if (customer == null)
        {
            customer = new()
            {
                Email = email,
                Processor = processorName,
                IsDefault = true
            };
            await _customerStore.CreateAsync(customer);
        }
        // TODO:
        // 1. Update all other paycustomers for this email/accountid to not be default.
        // https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
        return customer;
    }

    public Task<PayCustomer?> FindByEmailAsync(string email, string processorName)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Email == email && c.Processor == processorName));
    }

    public Task UpdateAsync(PayCustomer customer)
    {
        return _customerStore.UpdateAsync(customer);
    }
}