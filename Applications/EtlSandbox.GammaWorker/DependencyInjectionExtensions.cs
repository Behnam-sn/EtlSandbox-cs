using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.EtlApplicationStates.Repositories;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.EtlApplicationStates;
using EtlSandbox.Infrastructure.EtlApplicationStates.Repositories;
using EtlSandbox.Infrastructure.Shared;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Presentation.CustomerOrderFlats.Workers;
using EtlSandbox.Shared.ConfigureOptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.GammaWorker;

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
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly));
        services.AddScoped<IRequestHandler<InsertCommand<CustomerOrderFlat>>, InsertCommandHandler<CustomerOrderFlat>>();
        services.AddScoped<IRequestHandler<SoftDeleteCommand<CustomerOrderFlat>>, SoftDeleteCommandHandler<CustomerOrderFlat>>();
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework
        var connectionString = configuration.GetSection("DatabaseConnections")["Source"] ??
            throw new InvalidOperationException("Connection string 'Source'" + " not found.");

        services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
            connectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();
        services.AddScoped<IEtlApplicationStateCommandRepository, EtlApplicationStateSqlServerDapperCommandRepository>();
        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatEfExtractor>();
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatSqlServerBulkCopyLoader>();
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatSqlServerDapperSynchronizer>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertCustomerOrderFlatWorker>();
        services.AddHostedService<SoftDeleteCustomerOrderFlatWorker>();
    }
}