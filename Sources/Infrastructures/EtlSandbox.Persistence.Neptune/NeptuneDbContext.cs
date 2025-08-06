using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Persistence.Neptune;

public sealed class NeptuneDbContext(DbContextOptions<NeptuneDbContext> options) : DbContext(options)
{
    public DbSet<CustomerOrderFlat> CustomerOrderFlats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerOrderFlat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(50);
        });
    }
}