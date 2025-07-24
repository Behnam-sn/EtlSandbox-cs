using EtlSandbox.Application.ClickHouseUtils;
using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.ConfigureOptions;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Repositories;
using EtlSandbox.Infrastructure.Shared.UnitOfWorks;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.DeltaWebApi;

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
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IRequestHandler<GetCreateTableQuery, string>, GetCreateTableQueryHandler>();
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

        // Unit of Work
        services.AddScoped<IUnitOfWork, RawSqlUnitOfWork>();

        // Db Connection Factory
        services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();

        // Repositories
        services.AddScoped<IDatabaseRepository, SqlServerDapperDatabaseRepository>();
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