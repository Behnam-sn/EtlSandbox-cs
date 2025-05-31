using EtlSandbox.Domain;
using EtlSandbox.Infrastructure;
using EtlSandbox.Shared.Configurations;
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

        services.ConfigureOptions<ConnectionStringsSetup>();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        services.AddScoped<CustomerOrderFlatService>();
        services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddScoped<IExtractor<CustomerOrderFlat>, SqlExtractor>();
        // services.AddScoped<IExtractor<CustomerOrderFlat>, RestExtractor>();
        services.AddScoped<ILoader<CustomerOrderFlat>, SqlServerLoader>();

        services.AddHostedService<EtlWorker>();
    });

await builder.RunConsoleAsync();
