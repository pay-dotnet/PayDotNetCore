using Microsoft.EntityFrameworkCore;

namespace PayDotNet.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static void ApplyPayDotNetConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextExtensions).Assembly);
    }
}

public class PayChargeStore<TContext> : PayStoreBase<PayCharge, TContext>, IChargeStore
    where TContext : DbContext
{
    public PayChargeStore(TContext context) : base(context)
    {
    }

    public IQueryable<PayCharge> Charges => Entities;
}

public class PayCustomerStore<TContext> : PayStoreBase<PayCustomer, TContext>, ICustomerStore
    where TContext : DbContext
{
    public PayCustomerStore(TContext context) : base(context)
    {
    }

    public IQueryable<PayCustomer> Customers => Entities;
}

public class PayPaymentMethodStore<TContext> : PayStoreBase<PayPaymentMethod, TContext>, IPaymentMethodStore
    where TContext : DbContext
{
    public PayPaymentMethodStore(TContext context) : base(context)
    {
    }

    public IQueryable<PayPaymentMethod> PaymentMethods => Entities;
}

public class PaySubscriptionStore<TContext> : PayStoreBase<PaySubscription, TContext>, ISubscriptionStore
    where TContext : DbContext
{
    public PaySubscriptionStore(TContext context) : base(context)
    {
    }

    public IQueryable<PaySubscription> Subscriptions => Entities;
}

public class PayMerchantStore<TContext> : PayStoreBase<PayMerchant, TContext>, IMerchantStore
    where TContext : DbContext
{
    public PayMerchantStore(TContext context) : base(context)
    {
    }

    public IQueryable<PayMerchant> Merchants => Entities;
}