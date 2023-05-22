using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace PayDotNet.EntityFrameworkCore;

internal class DateTimeOffsetValueGeneratorFactory : ValueGenerator<DateTimeOffset>
{
    public override bool GeneratesTemporaryValues { get; }

    public override DateTimeOffset Next(EntityEntry entry)
    {
        if (entry == null) throw new ArgumentNullException(nameof(entry));
        return DateTimeOffset.UtcNow;
    }
}