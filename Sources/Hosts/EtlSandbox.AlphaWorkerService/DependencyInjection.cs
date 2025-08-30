using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Rentals;
using EtlSandbox.Infrastructure.Common.ConfigureOptions;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories.Destinations;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories.Sources;
using EtlSandbox.Infrastructure.Common.Repositories.Destinations;
using EtlSandbox.Infrastructure.Common.Resolvers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.Jupiter;
using EtlSandbox.Infrastructure.Jupiter.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.Mars;
using EtlSandbox.Infrastructure.Rentals.Repositories;
using EtlSandbox.Presentation.CustomerOrderFlats.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.AlphaWorkerService;

internal static class DependencyInjection
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<ConnectionStringsSetup>();
        services.ConfigureOptions<GlobalSettingsSetup>();
        services.ConfigureOptions<InsertWorkerSettingsSetup<RentalToCustomerOrderFlatsInsertWorker>>();
        services.ConfigureOptions<WorkerSettingsSetup<CustomerOrderFlatsSoftDeleteWorker>>();
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
        services.AddTransient<IRequestHandler<InsertCommand<Rental, CustomerOrderFlat>>, InsertCommandHandler<Rental, CustomerOrderFlat>>();
        services.AddTransient<IRequestHandler<SoftDeleteCommand<CustomerOrderFlat>>, SoftDeleteCommandHandler<CustomerOrderFlat>>();
    }

    internal static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Todo: Replace all comments with regions

        // Connection Strings
        var sourceConnectionString = configuration.GetConnectionString("Source") ??
            throw new InvalidOperationException("Connection string 'Source' not found.");
        var destinationConnectionString = configuration.GetConnectionString("Destination") ??
            throw new InvalidOperationException("Connection string 'Destination' not found.");

        // DbContexts
        services.AddDbContext<JupiterDbContext>(b => b.UseMySQL(
            sourceConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            })
        );
        services.AddDbContext<MarsDbContext>(b => b.UseSqlServer(
            destinationConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(Infrastructure.Mars.AssemblyReference.Assembly);
            })
        );

        // Db Connection Factories
        services.AddScoped<ISourceDbConnectionFactory, SourceMySqlConnectionFactory>();
        services.AddScoped<IDestinationDbConnectionFactory, DestinationSqlServerConnectionFactory>();

        // Source Repositories
        services.AddScoped<ISourceRepository<Rental>>(sp =>
        {
            var dbContext = sp.GetRequiredService<JupiterDbContext>();
            return new RentalEfSourceRepository(dbContext);
        });

        // Destination Repositories
        services.AddScoped<IDestinationRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<MarsDbContext>();
            return new EfDestinationRepositoryV1<CustomerOrderFlat>(dbContext);
        });

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatJupiterDapperExtractor>();

        // Transformers
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>>(_ => new CustomerOrderFlatSqlServerBulkCopyLoader(destinationConnectionString));

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatSqlServerDapperSynchronizer>();

        // Resolvers
        services.AddScoped(typeof(IInsertWorkerBatchSizeResolver<,,>), typeof(InsertWorkerBatchSizeResolver<,,>));
        services.AddScoped(typeof(IInsertWorkerDelayResolver<,,>), typeof(InsertWorkerDelayResolver<,,>));
        services.AddScoped(typeof(ISoftDeleteWorkerBatchSizeResolver<,>), typeof(SoftDeleteWorkerBatchSizeResolver<,>));
        services.AddScoped(typeof(ISoftDeleteWorkerDelayResolver<,>), typeof(SoftDeleteWorkerDelayResolver<,>));
        services.AddSingleton(typeof(IInsertStartingPointResolver<,>), typeof(InsertStartingPointResolver<,>));
        services.AddSingleton(typeof(ISoftDeleteStartingPointResolver<>), typeof(SoftDeleteStartingPointResolver<>));
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<RentalToCustomerOrderFlatsInsertWorker>();
        services.AddHostedService<CustomerOrderFlatsSoftDeleteWorker>();
    }
}