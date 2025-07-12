using EtlSandbox.Application.Shared.Commands;
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
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework
        var connectionString = configuration.GetSection("DatabaseConnections")["MySql"] ??
            throw new InvalidOperationException("Connection string 'SqlServer'" + " not found.");

        services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
            connectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IApplicationStateCommandRepository, ApplicationStateSqlServerDapperCommandRepository>();
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