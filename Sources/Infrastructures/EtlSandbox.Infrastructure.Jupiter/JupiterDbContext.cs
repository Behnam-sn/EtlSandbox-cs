using EtlSandbox.Domain.Rentals;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.Jupiter;

public sealed class JupiterDbContext(DbContextOptions<JupiterDbContext> options) : DbContext(options)
{
    public DbSet<Rental> Rental { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}