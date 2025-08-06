using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.Shared.ConfigureOptions;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Repositories;
using EtlSandbox.Infrastructure.Shared.Resolvers;
using EtlSandbox.Infrastructure.Shared.RestApiClients;
using EtlSandbox.Infrastructure.Shared.Transformers;
using EtlSandbox.Persistence.Neptune;
using EtlSandbox.Presentation.Shared.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.BetaWorkerService;

internal static class DependencyInjectionExtensions
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<ApplicationSettingsSetup>();
        services.ConfigureOptions<EntitySettingsSetup<CustomerOrderFlat>>();
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
        services
            .AddTransient<IRequestHandler<InsertCommand<CustomerOrderFlat, CustomerOrderFlat>>,
                InsertCommandHandler<CustomerOrderFlat, CustomerOrderFlat>>();
        services
            .AddTransient<IRequestHandler<SoftDeleteCommand<CustomerOrderFlat>>,
                SoftDeleteCommandHandler<CustomerOrderFlat>>();
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection Strings
        var sourceConnectionString = configuration.GetConnectionString("Source") ??
                                     throw new InvalidOperationException("Connection string 'Source' not found.");
        var destinationConnectionString = configuration.GetConnectionString("Destination") ??
                                          throw new InvalidOperationException(
                                              "Connection string 'Destination' not found.");

        // Entity Framework
        services.AddDbContext<NeptuneDbContext>(b => b.UseNpgsql(
            destinationConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(Persistence.Neptune.AssemblyReference.Assembly);
            })
        );

        // Repositories
        services.AddScoped<ISourceRepository<CustomerOrderFlat>, CustomerOrderFlatWebApiSourceRepository>();
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
        services.AddScoped<ITransformer<CustomerOrderFlat>, EmptyTransformer<CustomerOrderFlat>>();

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>>(_ =>
        {
            var connectionFactory = new NpgsqlConnectionFactory(destinationConnectionString);
            return new CustomerOrderFlatPostgreSqlDapperLoader(connectionFactory);
        });

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>>(_ =>
        {
            var connectionFactory = new NpgsqlConnectionFactory(destinationConnectionString);
            return new CustomerOrderFlatPostgreSqlDapperSynchronizer(connectionFactory);
        });

        // Resolvers
        services.AddSingleton(typeof(IInsertStartingPointResolver<,>), typeof(InsertStartingPointResolver<,>));
        services.AddSingleton(typeof(ISoftDeleteStartingPointResolver<>), typeof(SoftDeleteStartingPointResolver<>));

        // Rest Api Client
        services.AddHttpClient();
        services.AddScoped<IRestApiClient, FlurlRestApiClient>();
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertWorker<CustomerOrderFlat, CustomerOrderFlat>>();
        services.AddHostedService<SoftDeleteWorker<CustomerOrderFlat>>();
    }
}