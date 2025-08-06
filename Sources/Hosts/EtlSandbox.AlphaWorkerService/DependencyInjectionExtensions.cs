using EtlSandbox.Application.Shared.Commands;
using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Rentals;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.DbContexts;
using EtlSandbox.Infrastructure.Rentals;
using EtlSandbox.Infrastructure.Shared.ConfigureOptions;
using EtlSandbox.Infrastructure.Shared.DbConnectionFactories;
using EtlSandbox.Infrastructure.Shared.Repositories;
using EtlSandbox.Infrastructure.Shared.Resolvers;
using EtlSandbox.Presentation.Shared.Workers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.AlphaWorkerService;

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
        services.AddTransient<IRequestHandler<InsertCommand<Rental, CustomerOrderFlat>>, InsertCommandHandler<Rental, CustomerOrderFlat>>();
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
        services.AddDbContext<BDbContext>(b => b.UseMySQL(
            sourceConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            })
        );
        services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
            destinationConnectionString,
            providerOptions =>
            {
                providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                providerOptions.MigrationsAssembly(AssemblyReference.Assembly);
            })
        );

        // Repositories
        services.AddScoped<ISourceRepository<Rental>>(sp =>
        {
            var dbContext = sp.GetRequiredService<BDbContext>();
            return new RentalEfRepository(dbContext);
        });
        services.AddScoped<IDestinationRepository<CustomerOrderFlat>>(sp =>
        {
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();
            return new EfDestinationRepositoryV1<CustomerOrderFlat>(dbContext);
        });

        // Extractors
        services.AddScoped<IExtractor<CustomerOrderFlat>>(_ =>
        {
            var connectionFactory = new MySqlConnectionFactory(sourceConnectionString);
            return new CustomerOrderFlatMySqlDapperExtractor(connectionFactory);
        });

        // Transformers
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();

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
        services.AddHostedService<InsertWorker<Rental, CustomerOrderFlat>>();
        services.AddHostedService<SoftDeleteWorker<CustomerOrderFlat>>();
    }
}