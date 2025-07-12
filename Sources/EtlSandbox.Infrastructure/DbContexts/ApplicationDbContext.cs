using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.EtlApplicationStates.Entities;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<CustomerOrderFlat> CustomerOrders => Set<CustomerOrderFlat>();

    public DbSet<EtlApplicationState> EtlApplicationStates => Set<EtlApplicationState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerOrderFlat>(entity =>
        {
            entity.HasKey(e => e.RentalId);
            entity.Property(o => o.RentalId).ValueGeneratedNever();
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(50);
        });
    }
}