using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDotNet.Core.Models;

namespace PayDotNet.EntityFrameworkCore;

public class PayCustomerEntityTypeConfiguration : IEntityTypeConfiguration<PayCustomer>
{
    public void Configure(EntityTypeBuilder<PayCustomer> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.Processor, e.ProcessorId }).IsUnique();
        builder.Property(e => e.Email).IsRequired();
        builder.Property(e => e.IsDefault).IsRequired();
        builder.Property(e => e.Processor).IsRequired().HasMaxLength(512);

        // Relations
        builder.HasMany(e => e.Subscriptions).WithOne().HasForeignKey(e => e.CustomerId);
        builder.HasMany(e => e.PaymentMethods).WithOne().HasForeignKey(e => e.CustomerId);
        builder.HasMany(e => e.Charges).WithOne().HasForeignKey(e => e.CustomerId);

        // Other
        builder.Ignore(e => e.DefaultPaymentMethod);
    }
}