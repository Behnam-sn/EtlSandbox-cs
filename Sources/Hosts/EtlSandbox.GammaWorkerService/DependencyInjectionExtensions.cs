using EtlSandbox.Application.Common.Commands;
using EtlSandbox.Domain.Common;
using EtlSandbox.Domain.Common.Repositories;
using EtlSandbox.Domain.Common.Resolvers;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Infrastructure.Common.ConfigureOptions;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories;
using EtlSandbox.Infrastructure.Common.Repositories;
using EtlSandbox.Infrastructure.Common.Resolvers;
using EtlSandbox.Infrastructure.Common.Transformers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.Mars;
using EtlSandbox.Infrastructure.Venus;
using EtlSandbox.Presentation.Common.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.GammaWorkerService;

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
        services.AddDbContext<VenusDbContext>(b => b.UseSqlServer(
            destinationConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(Infrastructure.Venus.AssemblyReference.Assembly);
            })
        );

        // Source Repositories
        services.AddScoped<ISourceRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<MarsDbContext>();
            return new CustomerOrderFlatEfSourceRepository(dbContext);
        });

        // Destination Repositories
        services.AddScoped<IDestinationRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<VenusDbContext>();
            return new EfDestinationRepositoryV2<CustomerOrderFlat>(dbContext);
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
        services.AddScoped<ILoader<CustomerOrderFlat>>(_ => new CustomerOrderFlatSqlServerBulkCopyLoader(destinationConnectionString));

        // Synchronizers
        services.AddScoped<ISynchronizer<CustomerOrderFlat>>(_ =>
        {
            var connectionFactory = new SqlServerConnectionFactory(destinationConnectionString);
            return new CustomerOrderFlatSqlServerDapperSynchronizer(connectionFactory);
        });

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