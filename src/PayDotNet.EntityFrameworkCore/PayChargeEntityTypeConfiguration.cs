using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PayDotNet.EntityFrameworkCore;

public class PayChargeEntityTypeConfiguration : IEntityTypeConfiguration<PayCharge>
{
    public void Configure(EntityTypeBuilder<PayCharge> builder)
    {
        builder.HasKey(e => new { e.CustomerId, e.ProcessorId });
        builder.Property(e => e.Amount).IsRequired();
        builder.Property(e => e.AmountRefunded).IsRequired(false);
        builder.Property(e => e.ApplicationFeeAmount).IsRequired(false);
        builder.Property(e => e.Currency).IsRequired(false);
        builder.Property(e => e.ProcessorId).IsRequired();
        builder.Property(e => e.SubscriptionId).IsRequired(false);

        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();

        // Additional properties.
        builder.Property(e => e.Bank).IsRequired(false);
        builder.Property(e => e.Brand).IsRequired(false);
        builder.Property(e => e.Discounts).HasConversion<StringCollectionToStringValueConverter>();
        builder.Property(e => e.ExpirationMonth).IsRequired(false);
        builder.Property(e => e.ExpirationYear).IsRequired(false);
        builder.Property(e => e.InvoiceId).IsRequired(false);
        builder.Property(e => e.Last4).IsRequired(false);
        builder.Property(e => e.PaymentIntentId).IsRequired();
        builder.Property(e => e.PaymentMethodType).IsRequired();
        builder.Property(e => e.PeriodStart).IsRequired();
        builder.Property(e => e.PeriodEnd).IsRequired();
        builder.Property(e => e.ReceiptUrl).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.Subtotal).IsRequired();
        builder.Property(e => e.Tax).IsRequired(false);

        // Owner entities.
        builder.OwnsMany(e => e.LineItems, child =>
        {
            child.HasKey(e => e.ProcessorId);
            child.Property(e => e.Amount).IsRequired();
            child.Property(e => e.Description).IsRequired();
            child.Property(e => e.Amount).IsRequired();
            child.Property(e => e.IsProration).IsRequired();
            child.Property(e => e.PeriodStart).IsRequired();
            child.Property(e => e.PeriodEnd).IsRequired();
            child.Property(e => e.PriceId).IsRequired();
            child.Property(e => e.Quantity).IsRequired();
            child.Property(e => e.UnitAmount).IsRequired(false);

            child.OwnsMany(e => e.TaxAmounts, grandChild =>
            {
                grandChild.Property(e => e.Amount).IsRequired();
                grandChild.Property(e => e.Description).IsRequired();
            });

            child.Property(e => e.Discounts).HasConversion<StringCollectionToStringValueConverter>();
        });
        builder.OwnsMany(e => e.Refunds, child =>
        {
            child.Property(e => e.Amount).IsRequired();
            child.Property(e => e.CreatedAt).IsRequired();
            child.Property(e => e.Description).IsRequired();
            child.Property(e => e.ProcessorId).IsRequired();
            child.Property(e => e.Reason).IsRequired();
            child.Property(e => e.Status).IsRequired();
        });
        builder.OwnsMany(e => e.TotalDiscountAmounts, child =>
        {
            child.Property(e => e.Amount).IsRequired();
            child.Property(e => e.Description).IsRequired();
            child.Property(e => e.DiscountId).IsRequired();
        });
        builder.OwnsMany(e => e.TotalTaxAmounts, child =>
        {
            child.Property(e => e.Amount).IsRequired();
            child.Property(e => e.Description).IsRequired();
        });
    }
}