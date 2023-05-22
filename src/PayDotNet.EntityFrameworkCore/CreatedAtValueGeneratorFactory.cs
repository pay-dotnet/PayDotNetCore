using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace PayDotNet.EntityFrameworkCore;

internal class CreatedAtValueGeneratorFactory : ValueGenerator<DateTimeOffset>
{
    public override bool GeneratesTemporaryValues { get; }

    public override DateTimeOffset Next(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(nameof(entry));
        return DateTimeOffset.UtcNow;
    }
}

internal sealed class UpdatedAtValueGeneratorFactory : ValueGenerator<DateTimeOffset>
{
    public override bool GeneratesTemporaryValues { get; }

    public override DateTimeOffset Next(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(nameof(entry));
        return DateTimeOffset.UtcNow;
    }
}