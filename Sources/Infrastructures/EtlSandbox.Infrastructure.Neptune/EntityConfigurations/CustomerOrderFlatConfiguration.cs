using EtlSandbox.Domain.CustomerOrderFlats.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtlSandbox.Infrastructure.Neptune.EntityConfigurations;

internal sealed class CustomerOrderFlatConfiguration : IEntityTypeConfiguration<CustomerOrderFlat>
{
    public void Configure(EntityTypeBuilder<CustomerOrderFlat> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CustomerName).HasMaxLength(100);
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Category).HasMaxLength(50);
    }
}