using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayDotNet.Core.Models;

namespace PayDotNet.EntityFrameworkCore;

public class PayPaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PayPaymentMethod>
{
    public void Configure(EntityTypeBuilder<PayPaymentMethod> builder)
    {
        builder.HasKey(e => new { e.CustomerId, e.ProcessorId });
    }
}
