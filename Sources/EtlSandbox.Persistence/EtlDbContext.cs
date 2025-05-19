using EtlSandbox.Domain;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Persistence;

public class EtlDbContext : DbContext
{
    public EtlDbContext(DbContextOptions<EtlDbContext> options) : base(options)
    {
    }

    public DbSet<CustomerOrderFlat> CustomerOrders => Set<CustomerOrderFlat>();
    public DbSet<EtlState> EtlStates => Set<EtlState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerOrderFlat>(entity =>
        {
            entity.HasKey(e => e.RentalId);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(50);
        });
    }
}
