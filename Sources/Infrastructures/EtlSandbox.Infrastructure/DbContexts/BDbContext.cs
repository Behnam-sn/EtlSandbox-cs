using EtlSandbox.Domain.Rentals;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.DbContexts;

public sealed class BDbContext : DbContext
{
    public BDbContext(DbContextOptions<BDbContext> options) : base(options)
    {
    }
    
    public DbSet<Rental> Rental { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.rental_id);
        });
    }
}
