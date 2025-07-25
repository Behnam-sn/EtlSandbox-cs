using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.ConfigureOptions;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Repositories;
using EtlSandbox.Infrastructure.Shared.RestApiClients;
using EtlSandbox.Infrastructure.Shared.Synchronizers;
using EtlSandbox.Infrastructure.Shared.Transformers;

using EtlSandbox.Presentation.CustomerOrderFlats.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.BetaWorker;

internal static class DependencyInjectionExtensions
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<ApplicationSettingsSetup>();
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
        // Connection Strings
        var sourceConnectionString = configuration.GetConnectionString("Source") ??
                                          throw new InvalidOperationException("Connection string 'Source' not found.");
        var destinationConnectionString = configuration.GetConnectionString("Destination") ??
                                          throw new InvalidOperationException("Connection string 'Destination' not found.");

        // Entity Framework
        services.AddDbContext<ApplicationDbContext>(b => b.UseNpgsql(
            destinationConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        // Db Connection Factory
        services.AddScoped<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(destinationConnectionString));

        // Repositories
        services.AddScoped<IRepository<CustomerOrderFlat>, EfRepositoryV2<CustomerOrderFlat>>();

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>>(sp =>
        {
            var restApiClient = sp.GetRequiredService<IRestApiClient>();
            return new CustomerOrderFlatRestApiExtractor(sourceConnectionString, restApiClient);
        });

        // Transformers
        services.AddScoped<ITransformer<CustomerOrderFlat>, EmptyTransformer<CustomerOrderFlat>>();

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatPostgreSqlDapperLoader>();

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatPostgreSqlDapperSynchronizer>();
        services.AddSingleton<ISynchronizerUtils<CustomerOrderFlat>, SynchronizerUtils<CustomerOrderFlat>>();

        // Rest Api Client
        services.AddHttpClient();
        services.AddScoped<IRestApiClient, FlurlRestApiClient>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertCustomerOrderFlatWorker>();
        services.AddHostedService<SoftDeleteCustomerOrderFlatWorker>();
    }
}