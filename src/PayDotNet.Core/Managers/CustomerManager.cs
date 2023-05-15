using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Services;

namespace PayDotNet.Core.Managers;

public class CustomerManager : ICustomerManager
{
    private readonly ICustomerStore _customerStore;
    private readonly IPaymentProcessorService _paymentProcessorService;

    public CustomerManager(
        CompositePaymentProcessorService paymentProcessorService,
        ICustomerStore customerStore)
    {
        _paymentProcessorService = paymentProcessorService;
        _customerStore = customerStore;
    }

    /// <inheritdoc/>
    public virtual Task<PayCustomer?> FindByEmailAsync(string processorName, string email)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Email == email && c.Processor == processorName));
    }

    /// <inheritdoc/>
    public virtual Task<PayCustomer?> FindByIdAsync(string processorName, string processorId)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.ProcessorId == processorId));
    }

    /// <inheritdoc/>
    public virtual async Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string processorId, string email)
    {
        PayCustomer? customer = _customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.ProcessorId == processorId);
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

    /// <inheritdoc/>
    public virtual async Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string email)
    {
        PayCustomer? payCustomer = _customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.Email == email);
        if (payCustomer == null)
        {
            payCustomer = new()
            {
                Email = email,
                Processor = processorName,
                IsDefault = true
            };
            await _customerStore.CreateAsync(payCustomer);
        }

        if (!payCustomer.HasProcessorId())
        {
            string processorId = await _paymentProcessorService.CreateCustomerAsync(payCustomer);
            payCustomer.ProcessorId = processorId;
            await _customerStore.UpdateAsync(payCustomer);
        }

        // TODO:
        // 1. Update all other paycustomers for this email/accountid to not be default.
        // https://github.com/pay-rails/pay/blob/75cebad8786c901a447cc6459174c7044e2b261d/lib/pay/attributes.rb#L29
        return payCustomer;
    }

    /// <inheritdoc/>
    public virtual Task SoftDeleteAsync(PayCustomer payCustomer)
    {
        payCustomer.DeletedAt = DateTime.UtcNow;
        payCustomer.IsDefault = false;
        return _customerStore.UpdateAsync(payCustomer);
    }
}