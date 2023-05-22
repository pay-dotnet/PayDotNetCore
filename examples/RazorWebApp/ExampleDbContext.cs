using Microsoft.EntityFrameworkCore;
using PayDotNet.EntityFrameworkCore;

namespace RazorWebApp;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyPayDotNetConfigurations();
        base.OnModelCreating(modelBuilder);
    }
}