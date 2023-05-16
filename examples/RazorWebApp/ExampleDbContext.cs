using Microsoft.EntityFrameworkCore;
using PayDotNet.EntityFrameworkCore;
using PayDotNet.Stores;

namespace RazorWebApp;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<PayCharge> Charges { get; set; }

    public DbSet<PayCustomer> Customers { get; set; }

    public DbSet<PayPaymentMethod> PaymentMethods { get; set; }

    public DbSet<PaySubscription> Subscriptions { get; set; }

    public DbSet<PayWebhook> Webhooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyPayDotNetConfigurations();
        base.OnModelCreating(modelBuilder);
    }
}