using Microsoft.EntityFrameworkCore;
using PayDotNet.EntityFrameworkCore;

namespace EntityFramework.Migrations;

internal class MyPayDbContext : DbContext
{
    public MyPayDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "paydotnet.db");
    }

    public string DbPath { get; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyPayDotNetConfigurations();
        base.OnModelCreating(modelBuilder);
    }
}