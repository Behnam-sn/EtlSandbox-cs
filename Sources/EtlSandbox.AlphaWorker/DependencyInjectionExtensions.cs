using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.ApplicationStates;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared;
using EtlSandbox.Infrastructure.Shared.RestApiClients;
using EtlSandbox.Presentation.CustomerOrderFlats.Workers;
using EtlSandbox.Shared.ConfigureOptions;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.AlphaWorker;

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
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        // Rest Api Client
        services.AddHttpClient();
        services.AddScoped<IRestApiClient, FlurlRestApiClient>();

        services.AddScoped<IApplicationStateCommandRepository, ApplicationStateDapperCommandRepository>();
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatRestApiExtractor>();
        services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatSqlBulkCopyLoader>();
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatDapperSynchronizer>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertCustomerOrderFlatWorker>();
        services.AddHostedService<SoftDeleteCustomerOrderFlatWorker>();
    }
}