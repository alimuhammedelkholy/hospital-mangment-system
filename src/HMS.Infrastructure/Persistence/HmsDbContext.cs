using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Persistence;

/// <summary>SQL Server context reserved for mappings derived from the approved database schema.</summary>
public sealed class HmsDbContext(DbContextOptions<HmsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HmsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
