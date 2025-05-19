using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EtlSandbox.Persistence;

public class EtlDbContextFactory : IDesignTimeDbContextFactory<EtlDbContext>
{
    public EtlDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EtlDbContext>()
            .UseSqlServer("Server=localhost,3496;Database=YourDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;")
            .Options;

        return new EtlDbContext(options);
    }
}
