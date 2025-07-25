using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.ConfigureOptions;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Repositories;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

using EtlSandbox.Presentation.CustomerOrderFlats.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.AlphaWorker;

internal static class DependencyInjectionExtensions
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<ApplicationSettingsSetup>();
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
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IRequestHandler<InsertCommand<CustomerOrderFlat>>, InsertCommandHandler<CustomerOrderFlat>>();
        services.AddScoped<IRequestHandler<SoftDeleteCommand<CustomerOrderFlat>>, SoftDeleteCommandHandler<CustomerOrderFlat>>();
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework
        var connectionString = configuration.GetSection("DatabaseConnections")["Destination"] ??
            throw new InvalidOperationException("Connection string 'Destination'" + " not found.");

        services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
            connectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );



        // Db Connection Factory
        services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();

        // Repositories
        services.AddScoped<IRepository<CustomerOrderFlat>, EfRepositoryV1<CustomerOrderFlat>>();

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatMySqlDapperExtractor>();

        // Transformers
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatSqlServerBulkCopyLoader>();

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatSqlServerDapperSynchronizer>();
        services.AddSingleton<ISynchronizerUtils<CustomerOrderFlat>, SynchronizerUtils<CustomerOrderFlat>>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertCustomerOrderFlatWorker>();
        // services.AddHostedService<SoftDeleteCustomerOrderFlatWorker>();
    }
}