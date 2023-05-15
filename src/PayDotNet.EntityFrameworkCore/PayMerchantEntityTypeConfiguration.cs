using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayDotNet.EntityFrameworkCore;

public class PayMerchantEntityTypeConfiguration : IEntityTypeConfiguration<PayMerchant>
{
    public void Configure(EntityTypeBuilder<PayMerchant> builder)
    {
        builder.HasKey(e => e.ProcessorId);
        builder.Property(e => e.Processor).IsRequired();
        builder.Property(e => e.IsDefault).IsRequired();
        builder.Property(e => e.IsOnboardingComplete).IsRequired();
    }
}