using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Shared.ConfigureOptions;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Resolvers;
using EtlSandbox.Infrastructure.Shared.Transformers;
using EtlSandbox.Presentation.Shared.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.DeltaWorkerService;

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

        // Entity Framework
        services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
            sourceConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        // Db Connection Factory
        services.AddScoped<IDbConnectionFactory>(_ => new ClickHouseConnectionFactory(destinationConnectionString));

        // Repositories
        services.AddScoped<ISourceRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();
            return new CustomerOrderFlatEfSourceRepository(dbContext);
        });
        services.AddScoped<IDestinationRepository<CustomerOrderFlat>, CustomerOrderFlatClickHouseDapperDestinationRepository>();

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatEfExtractor>();

        // Transformers
        services.AddScoped(typeof(ITransformer<>), typeof(EmptyTransformer<>));

        // Loaders
        services.AddScoped<ILoader<CustomerOrderFlat>>(_ => new CustomerOrderFlatClickHouseBulkCopyLoader(destinationConnectionString));

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>, CustomerOrderFlatClickHouseDapperSynchronizer>();

        // Resolvers
        services.AddSingleton(typeof(IInsertStartingPointResolver<,>), typeof(InsertStartingPointResolver<,>));
        services.AddSingleton(typeof(ISoftDeleteStartingPointResolver<>), typeof(SoftDeleteStartingPointResolver<>));
    }

    internal static void AddPresentation(this IServiceCollection services)
    {
        services.AddHostedService<InsertWorker<CustomerOrderFlat, CustomerOrderFlat>>();
        services.AddHostedService<SoftDeleteWorker<CustomerOrderFlat>>();
    }
}