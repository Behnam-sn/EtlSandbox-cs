using EtlSandbox.Domain;
using EtlSandbox.Infrastructure;
using EtlSandbox.Shared;
using EtlSandbox.Shared.Configurations;
using EtlSandbox.Worker;
using EtlSandbox.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddSharedConfiguration();

builder.Services.AddHttpClient();

builder.Services.ConfigureOptions<ConnectionStringsSetup>();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddScoped<IEtlStateCommandRepository, EtlStateCommandRepository>();
builder.Services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
// builder.Services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatDbExtractor>();
builder.Services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatRestApiExtractor>();
builder.Services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatSqlServerLoader>();

builder.Services.AddHostedService<InsertCustomerOrderFlatWorker>();

var host = builder.Build();
await host.RunAsync();