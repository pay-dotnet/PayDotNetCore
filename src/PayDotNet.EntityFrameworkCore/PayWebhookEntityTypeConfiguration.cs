using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayDotNet.EntityFrameworkCore;

public class PayWebhookEntityTypeConfiguration : IEntityTypeConfiguration<PayWebhook>
{
    public void Configure(EntityTypeBuilder<PayWebhook> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Event).IsRequired();
        builder.Property(e => e.EventType).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}