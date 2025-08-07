using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Mars;

public sealed class MarsDbContext(DbContextOptions<MarsDbContext> options) : DbContext(options)
{
    public DbSet<CustomerOrderFlat> CustomerOrderFlats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}