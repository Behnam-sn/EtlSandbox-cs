using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Shared.ConfigureOptions;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.WebApi;

internal static class DependencyInjectionExtensions
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<DatabaseConnectionsSetup>();
    }

    internal static void AddLogs(this IServiceCollection services)
    {
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
    }

    public static void AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly));
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework
        var connectionString = configuration.GetSection("DatabaseConnections")["SqlServer"] ??
            throw new InvalidOperationException("Connection string 'SqlServer'" + " not found.");

        services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
            connectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatEfExtractor>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
    }
}