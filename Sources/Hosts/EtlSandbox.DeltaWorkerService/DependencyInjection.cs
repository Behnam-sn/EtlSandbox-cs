using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.ConfigureOptions;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories;
using EtlSandbox.Infrastructure.Common.Resolvers;
using EtlSandbox.Infrastructure.Common.Transformers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Destinations;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories.Sources;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.Mars;
using EtlSandbox.Presentation.CustomerOrderFlats.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.DeltaWorkerService;

internal static class DependencyInjection
{
    internal static void AddConfigureOptions(this IServiceCollection services)
    {
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
            throw new InvalidOperationException("Connection string 'Destination' not found.");

        // DbContexts
        services.AddDbContext<MarsDbContext>(b => b.UseSqlServer(
            sourceConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            })
        );

        // Source Repositories
        services.AddScoped<ISourceRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<MarsDbContext>();
            return new CustomerOrderFlatEfSourceRepository(dbContext);
        });

        // Destination Repositories
        services.AddScoped<IDestinationRepository<CustomerOrderFlat>>(_ =>
        {
            var connectionFactory = new ClickHouseConnectionFactory(destinationConnectionString);
            return new CustomerOrderFlatClickHouseDapperDestinationRepository(connectionFactory);
        });

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<MarsDbContext>();
            return new CustomerOrderFlatEfExtractor(dbContext);
        });

        // Transformers
        services.AddScoped(typeof(ITransformer<>), typeof(EmptyTransformer<>));

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>>(_ => new CustomerOrderFlatClickHouseBulkCopyLoader(destinationConnectionString));

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>>(_ =>
        {
            var connectionFactory = new ClickHouseConnectionFactory(destinationConnectionString);
            return new CustomerOrderFlatClickHouseDapperSynchronizer(connectionFactory);
        });

        // Resolvers
        services.AddSingleton(typeof(IInsertStartingPointResolver<,>), typeof(InsertStartingPointResolver<,>));
        services.AddSingleton(typeof(ISoftDeleteStartingPointResolver<>), typeof(SoftDeleteStartingPointResolver<>));
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<CustomerOrderFlatsToCustomerOrderFlatsInsertWorker>();
        services.AddHostedService<CustomerOrderFlatsSoftDeleteWorker>();
    }
}