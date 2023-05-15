using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDotNet.Core.Models;

namespace PayDotNet.EntityFrameworkCore;

public class PaySubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<PaySubscription>
{
    public void Configure(EntityTypeBuilder<PaySubscription> builder)
    {
        builder.HasKey(e => new { e.CustomerId, e.ProcessorId });
        builder.OwnsMany(e => e.SubscriptionItems, child =>
        {
            child.HasKey(e => e.Id);
            child.Property(e => e.Quantity).IsRequired();
            child.OwnsOne(e => e.Price, grandChild =>
            {
                grandChild.HasKey(e => e.Id);
            });
            child.HasKey(e => e.Id);
        });

        builder.Property(e => e.ApplicationFeePercent).HasPrecision(8, 2);
    }
}