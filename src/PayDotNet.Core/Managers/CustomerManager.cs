using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class CustomerManager : ICustomerManager
{
    private readonly IPaymentProcessorService _paymentProcessorService;
    private readonly ICustomerStore _customerStore;

    public CustomerManager(
        IPaymentProcessorService paymentProcessorService,
        ICustomerStore customerStore)
    {
        _paymentProcessorService = paymentProcessorService;
        _customerStore = customerStore;
    }

    public async Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string processorId, string email)
    {
        PayCustomer? customer = _customerStore.Customers.FirstOrDefault(c => c.ProcessorId == processorId && c.Processor == processorName);
        if (customer == null)
        {
            customer = new()
            {
                Email = email,
                Processor = processorName,
                ProcessorId = processorId,
                IsDefault = true
            };
            await _customerStore.CreateAsync(customer);
        }
        return customer;
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
        await InitializeCustomerAsync(customer);

        // TODO:
        // 1. Update all other paycustomers for this email/accountid to not be default.
        // https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
        return customer;
    }

    private async Task InitializeCustomerAsync(PayCustomer payCustomer)
    {
        if (!payCustomer.HasProcessorId())
        {
            PaymentProcessorCustomer customer = await _paymentProcessorService.CreateCustomerAsync(payCustomer.Email, new());
            payCustomer.ProcessorId = customer.Id;
            await _customerStore.UpdateAsync(payCustomer);
        }
    }

    public Task<PayCustomer?> FindByEmailAsync(string email, string processorName)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Email == email && c.Processor == processorName));
    }

    public Task UpdateAsync(PayCustomer customer)
    {
        return _customerStore.UpdateAsync(customer);
    }

    public Task<PayCustomer?> FindByIdAsync(string processorName, string processorId)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.ProcessorId == processorId));
    }
}