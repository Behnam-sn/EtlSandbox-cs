using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EtlSandbox.Persistence;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlServer("Server=localhost;Database=YourDb;Trusted_Connection=True;")
            .Options;

        return new ApplicationContext(options);
    }
}
