using EtlSandbox.Domain;
using EtlSandbox.Domain.Configurations;
using EtlSandbox.Infrastructure;
using EtlSandbox.Worker;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // var config = hostContext.Configuration;
        // var sqlServerConnection = config.GetConnectionString("SqlServer")!;

        // services.AddDbContext<EtlDbContext>(options =>
        //     options.UseSqlServer(sqlServerConnection)
        // );

        services.AddHttpClient();

        services.Configure<ConnectionStrings>(
            hostContext.Configuration.GetSection("ConnectionStrings")
        );

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddSingleton<IExtractor<CustomerOrderFlat>, RestExtractor>();
        services.AddSingleton<ILoader<CustomerOrderFlat>, SqlServerLoader>();

        services.AddHostedService<EtlWorker>();
    });

await builder.RunConsoleAsync();
