using EtlSandbox.Domain;
using EtlSandbox.Infrastructure;
using EtlSandbox.Worker;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var config = hostContext.Configuration;
        var mysqlConnection = config.GetConnectionString("MySql")!;
        var sqlServerConnection = config.GetConnectionString("SqlServer")!;

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddSingleton<IExtractor<CustomerOrderFlat>>(sp => new SqlExtractor(mysqlConnection, sp.GetRequiredService<ILogger<SqlExtractor>>()));
        services.AddSingleton<ILoader<CustomerOrderFlat>>(sp => new SqlServerLoader(sqlServerConnection, sp.GetRequiredService<ILogger<SqlServerLoader>>()));

        services.AddHostedService<EtlWorker>();
    });

await builder.RunConsoleAsync();

// var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<Worker>();

// var host = builder.Build();
// host.Run();
