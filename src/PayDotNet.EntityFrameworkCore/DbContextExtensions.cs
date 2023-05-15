using Microsoft.EntityFrameworkCore;

namespace PayDotNet.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static void ApplyPayDotNetConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextExtensions).Assembly);
    }
}