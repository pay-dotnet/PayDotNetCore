using PayDotNet.Core.Abstraction;
using PayDotNet.Core.Models;

namespace PayDotNet.Core.Stores;

internal class InMemoryStore :
    ICustomerStore,
    IChargeStore,
    IPaymentMethodStore,
    ISubscriptionStore
{
    public static readonly Dictionary<string, PayCustomer> Data = new();

    public IQueryable<PaySubscription> Subscriptions => Data.Values.SelectMany(c => c.Subscriptions).AsQueryable();

    public IQueryable<PayCustomer> Customers => Data.Values.AsQueryable();

    public IQueryable<PayPaymentMethod> PaymentMethods => Data.Values.SelectMany(c => c.PaymentMethods).AsQueryable();

    public IQueryable<PayCharge> Charges => Data.Values.SelectMany(c => c.Charges).AsQueryable();

    public Task CreateAsync(PaySubscription model)
    {
        Data[model.CustomerId].Subscriptions.Add(model);
        return Task.CompletedTask;
    }

    public Task CreateAsync(PayCustomer model)
    {
        Data.Add(model.Id, model);
        return Task.CompletedTask;
    }

    public Task CreateAsync(PayPaymentMethod model)
    {
        Data[model.CustomerId].PaymentMethods.Add(model);
        return Task.CompletedTask;
    }

    public Task CreateAsync(PayCharge model)
    {
        Data[model.CustomerId].Charges.Add(model);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PaySubscription model)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ICollection<PaySubscription> models)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PayCustomer model)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ICollection<PayCustomer> models)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PayPaymentMethod model)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ICollection<PayPaymentMethod> models)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PayCharge model)
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ICollection<PayCharge> models)
    {
        return Task.CompletedTask;
    }
}