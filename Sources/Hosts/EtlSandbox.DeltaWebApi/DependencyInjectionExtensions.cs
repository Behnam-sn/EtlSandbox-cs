using EtlSandbox.Application.ClickHouseUtils;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Repositories;
using EtlSandbox.Persistence.Jupiter;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.DeltaWebApi;

internal static class DependencyInjectionExtensions
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

    public static void AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddTransient<IMediator, Mediator>();
        services.AddTransient<IRequestHandler<GetCreateTableQuery, string>, GetCreateTableQueryHandler>();
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection Strings
        var sourceConnectionString = configuration.GetConnectionString("Source") ??
            throw new InvalidOperationException("Connection string 'Source' not found.");

        // Entity Framework
        services.AddDbContext<JupiterDbContext>(b => b.UseSqlServer(
            sourceConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            })
        );

        // Repositories
        services.AddScoped<IDatabaseRepository>(_ =>
        {
            var connectionFactory = new SqlServerConnectionFactory(sourceConnectionString);
            return new SqlServerDapperDatabaseRepository(connectionFactory);
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