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
    public virtual async Task<PayCustomer> FindByIdAsync(string processorName, string processorId)
    {
        PayCustomer? payCustomer = await TryFindByIdAsync(processorName, processorId);
        if (payCustomer is null)
        {
            throw new PayDotNetException(string.Format("PayCustomer for payment processor {0} does not exist for processor id '{1}'. ", processorId, processorName));
        }
        return payCustomer;
    }

    /// <inheritdoc/>
    public virtual async Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string processorId, string email)
    {
        PayCustomer? payCustomer = _customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.ProcessorId == processorId);
        if (payCustomer == null)
        {
            payCustomer = await CreateCustomerAsync(email, processorName, processorId);
        }

        await SetPayCustomersIsDefaultAsync(payCustomer);
        return payCustomer;
    }

    /// <inheritdoc/>
    public virtual async Task<PayCustomer> GetOrCreateCustomerAsync(string processorName, string email)
    {
        PayCustomer? payCustomer = _customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.Email == email);
        if (payCustomer == null)
        {
            payCustomer = await CreateCustomerAsync(email, processorName, processorId: null);
        }

        if (!payCustomer.HasProcessorId())
        {
            PayCustomerResult result = await _paymentProcessorService.CreateCustomerAsync(payCustomer);
            payCustomer.ProcessorId = result.ProcessorId;
            payCustomer.Account = result.Account;
        }

        await SetPayCustomersIsDefaultAsync(payCustomer);
        return payCustomer;
    }

    /// <inheritdoc/>
    public virtual Task SoftDeleteAsync(PayCustomer payCustomer)
    {
        payCustomer.DeletedAt = DateTime.UtcNow;
        payCustomer.IsDefault = false;
        return _customerStore.UpdateAsync(payCustomer);
    }

    /// <inheritdoc/>
    public virtual Task<PayCustomer?> TryFindByEmailAsync(string processorName, string email)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Email == email && c.Processor == processorName));
    }

    /// <inheritdoc/>
    public Task<PayCustomer?> TryFindByIdAsync(string processorName, string processorId)
    {
        return Task.FromResult(_customerStore.Customers.FirstOrDefault(c => c.Processor == processorName && c.ProcessorId == processorId));
    }

    private async Task<PayCustomer> CreateCustomerAsync(string email, string processorName, string? processorId)
    {
        PayCustomer payCustomer = new()
        {
            Email = email,
            Processor = processorName,
            ProcessorId = processorId,
            IsDefault = true
        };
        await _customerStore.CreateAsync(payCustomer);
        return payCustomer;
    }

    /// <remarks>
    /// This has several effects:
    /// - Updates the PayCustomer for the process and marks it as default
    /// - Removes the default flag from all other PayCustomers
    ///
    /// This is only done when the IsDefault on the PayCustomer is not set yet.
    /// </remarks>
    private async Task SetPayCustomersIsDefaultAsync(PayCustomer payCustomer)
    {
        if (payCustomer.IsDefault)
        {
            return;
        }

        ICollection<PayCustomer> otherPayCustomersForEmail = _customerStore.Customers.Where(c => c.Email != payCustomer.Email).ToList();
        foreach (PayCustomer otherPayCustomer in otherPayCustomersForEmail)
        {
            otherPayCustomer.IsDefault = false;
        }

        payCustomer.IsDefault = true;
        await _customerStore.UpdateAllAsync(new List<PayCustomer>(otherPayCustomersForEmail)
        {
            payCustomer
        });
    }
}