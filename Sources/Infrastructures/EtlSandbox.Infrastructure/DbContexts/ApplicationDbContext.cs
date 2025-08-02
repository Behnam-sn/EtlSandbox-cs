using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.DbContexts;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<CustomerOrderFlat> CustomerOrderFlats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerOrderFlat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(50);
        });
    }
}