using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.Shared.ApiClient;
using EtlSandbox.Persistence;
using EtlSandbox.Shared.ConfigureOptions;
using EtlSandbox.Worker.CustomerOrderFlats.Workers;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Worker;

internal static class DependencyInjectionExtensions
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<DatabaseConnectionsSetup>();
        services.ConfigureOptions<RestApiConnectionsSetup>();
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
            bb => { bb.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); })
        );

        // Rest Api Client
        services.AddHttpClient();
        services.AddScoped<IRestApiClient, FlurlRestApiClient>();

        services.AddScoped<IEtlStateCommandRepository, EtlStateCommandRepository>();
        services.AddScoped<ICommandRepository<CustomerOrderFlat>, CustomerOrderFlatEfCommandRepository>();
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatRestApiExtractor>();
        services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatSqlBulkCopyLoader>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertCustomerOrderFlatWorker>();
    }
}