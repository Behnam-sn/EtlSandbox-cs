using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Venus;

public sealed class VenusDbContext(DbContextOptions<VenusDbContext> options) : DbContext(options)
{
    public DbSet<CustomerOrderFlat> CustomerOrderFlats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}