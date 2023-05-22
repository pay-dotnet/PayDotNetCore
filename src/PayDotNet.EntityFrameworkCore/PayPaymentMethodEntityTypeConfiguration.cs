using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayDotNet.EntityFrameworkCore;

public class PayPaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PayPaymentMethod>
{
    public void Configure(EntityTypeBuilder<PayPaymentMethod> builder)
    {
        builder.HasKey(e => new { e.CustomerId, e.ProcessorId });

        builder.Property(e => e.IsDefault).IsRequired();
        builder.Property(e => e.Type).IsRequired();

        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}