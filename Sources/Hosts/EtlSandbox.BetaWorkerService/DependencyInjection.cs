using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.ConfigureOptions;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories.Destinations;
using EtlSandbox.Infrastructure.Common.Repositories.Destinations;
using EtlSandbox.Infrastructure.Common.Resolvers;
using EtlSandbox.Infrastructure.Common.RestApiClients;
using EtlSandbox.Infrastructure.Common.Transformers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Sources;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.Neptune;
using EtlSandbox.Presentation.CustomerOrderFlats.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.BetaWorkerService;

internal static class DependencyInjection
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<ConnectionStringsSetup>();
        services.ConfigureOptions<GlobalSettingsSetup>();
        services.ConfigureOptions<InsertWorkerSettingsSetup<CustomerOrderFlatsToCustomerOrderFlatsInsertWorker>>();
        services.ConfigureOptions<SoftDeleteWorkerSettingsSetup<CustomerOrderFlatsSoftDeleteWorker>>();
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
        // Mediatr
        services.AddTransient<IMediator, Mediator>();
        services.AddTransient<IRequestHandler<InsertCommand<CustomerOrderFlat, CustomerOrderFlat>>, InsertCommandHandler<CustomerOrderFlat, CustomerOrderFlat>>();
        services.AddTransient<IRequestHandler<SoftDeleteCommand<CustomerOrderFlat>>, SoftDeleteCommandHandler<CustomerOrderFlat>>();
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection Strings
        var sourceConnectionString = configuration.GetConnectionString("Source") ??
            throw new InvalidOperationException("Connection string 'Source' not found.");
        var destinationConnectionString = configuration.GetConnectionString("Destination") ??
            throw new InvalidOperationException(
                "Connection string 'Destination' not found.");

        // DbContexts
        services.AddDbContext<NeptuneDbContext>(b => b.UseNpgsql(
            destinationConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(Infrastructure.Neptune.AssemblyReference.Assembly);
            })
        );

        // Db Connection Factories
        services.AddScoped<IDestinationDbConnectionFactory, DestinationNpgsqlConnectionFactory>();

        // Source Repositories
        services.AddScoped<ISourceRepository<CustomerOrderFlat>>(sp =>
        {
            var restApiClient = sp.GetRequiredService<IRestApiClient>();
            return new CustomerOrderFlatWebApiSourceRepository(sourceConnectionString, restApiClient);
        });

        // Destination Repositories
        services.AddScoped<IDestinationRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<NeptuneDbContext>();
            return new EfDestinationRepositoryV2<CustomerOrderFlat>(dbContext);
        });

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>>(sp =>
        {
            var restApiClient = sp.GetRequiredService<IRestApiClient>();
            return new CustomerOrderFlatRestApiExtractor(sourceConnectionString, restApiClient);
        });

        // Transformers
        services.AddScoped(typeof(ITransformer<>), typeof(EmptyTransformer<>));

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>>(sp =>
        {
            var connectionFactory = sp.GetRequiredService<IDestinationDbConnectionFactory>();
            return new CustomerOrderFlatPostgreSqlDapperLoader(connectionFactory);
        });

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatPostgreSqlDapperSynchronizer>();

        // Resolvers
        services.AddScoped(typeof(IInsertWorkerBatchSizeResolver<,,>), typeof(InsertWorkerBatchSizeResolver<,,>));
        services.AddSingleton(typeof(IInsertStartingPointResolver<,>), typeof(InsertStartingPointResolver<,>));
        services.AddSingleton(typeof(ISoftDeleteStartingPointResolver<>), typeof(SoftDeleteStartingPointResolver<>));

        // Rest Api Client
        services.AddHttpClient();
        services.AddScoped<IRestApiClient, FlurlRestApiClient>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<CustomerOrderFlatsToCustomerOrderFlatsInsertWorker>();
        services.AddHostedService<CustomerOrderFlatsSoftDeleteWorker>();
    }
}