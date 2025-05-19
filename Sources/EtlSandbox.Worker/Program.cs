using EtlSandbox.Domain;
using EtlSandbox.Infrastructure;
using EtlSandbox.Worker;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var config = hostContext.Configuration;
        var mysqlConnection = config.GetConnectionString("MySql")!;
        var sqlServerConnection = config.GetConnectionString("SqlServer")!;

        services.AddSingleton<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
        services.AddSingleton<IExtractor<CustomerOrderFlat>>(new SqlExtractor(mysqlConnection));
        services.AddSingleton<ILoader<CustomerOrderFlat>>(new SqlServerLoader(sqlServerConnection));
        services.AddHostedService<EtlWorker>();
    });

await builder.RunConsoleAsync();

// var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddHostedService<Worker>();

// var host = builder.Build();
// host.Run();
