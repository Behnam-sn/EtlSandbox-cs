using EtlSandbox.Domain;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Persistence;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<CustomerOrderFlat> CustomerOrders => Set<CustomerOrderFlat>();

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
