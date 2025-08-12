using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.Mars;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.BetaWebApiService;

internal static class DependencyInjection
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
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

    internal static void AddApplication(this IServiceCollection services)
    {
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection Strings
        var sourceConnectionString = configuration.GetConnectionString("Source") ??
            throw new InvalidOperationException("Connection string 'Source' not found.");

        // DbContexts
        services.AddDbContext<MarsDbContext>(b => b.UseSqlServer(
            sourceConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            })
        );

        // Source Repositories
        services.AddScoped<ISourceRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<MarsDbContext>();
            return new CustomerOrderFlatEfSourceRepository(dbContext);
        });

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<MarsDbContext>();
            return new CustomerOrderFlatEfExtractor(dbContext);
        });
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddHealthChecks();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}