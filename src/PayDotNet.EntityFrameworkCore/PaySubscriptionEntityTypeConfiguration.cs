using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayDotNet.EntityFrameworkCore;

public class PaySubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<PaySubscription>
{
    public void Configure(EntityTypeBuilder<PaySubscription> builder)
    {
        builder.HasKey(e => new { e.CustomerId, e.ProcessorId });

        builder.Property(e => e.ApplicationFeePercent).HasPrecision(8, 2);
        builder.Property(e => e.CurrentPeriodStart).IsRequired(false);
        builder.Property(e => e.CurrentPeriodEnd).IsRequired(false);
        builder.Property(e => e.IsMetered).IsRequired();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.PauseBehaviour).IsRequired(false);
        builder.Property(e => e.PauseResumesAt).IsRequired(false);
        builder.Property(e => e.PauseStartsAt).IsRequired(false);
        builder.Property(e => e.ProcessorPlan).IsRequired();
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.TrailEndsAt).IsRequired(false);

        builder.Property(e => e.CreatedAt).IsRequired().ValueGeneratedOnAdd().HasValueGenerator<DateTimeOffsetValueGeneratorFactory>();
        builder.Property(e => e.UpdatedAt).IsRequired().ValueGeneratedOnUpdate().HasValueGenerator<DateTimeOffsetValueGeneratorFactory>();

        // Owned entities.
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
    }
}