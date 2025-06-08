using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EtlSandbox.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost,5002;Database=SakilaFlat;User Id=sa;Password=Your_password123;TrustServerCertificate=True;")
            .Options;

        return new ApplicationDbContext(options);
    }
}
