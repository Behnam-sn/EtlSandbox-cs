using EtlSandbox.Domain;
using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Loaders;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Repositories;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Transformers;
using EtlSandbox.Infrastructure.Shared.ApiClient;
using EtlSandbox.Persistence;
using EtlSandbox.Shared;
using EtlSandbox.Shared.Configurations;
using EtlSandbox.Worker.CustomerOrderFlats.Workers;

using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddSharedConfiguration();

builder.Services.AddHttpClient();

builder.Services.ConfigureOptions<ConnectionStringsSetup>();

var connectionString = builder.Configuration.GetConnectionString("SqlServer") ??
    throw new InvalidOperationException("Connection string 'SqlServer'" + " not found.");

builder.Services.AddDbContext<ApplicationDbContext>(b => b.UseSqlServer(
    connectionString,
    bb => { bb.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); })
);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddScoped<IApiClient, FlurlApiClient>();

builder.Services.AddScoped<IEtlStateCommandRepository, EtlStateCommandRepository>();
builder.Services.AddScoped<ICommandRepository<CustomerOrderFlat>, CustomerOrderFlatEfCommandRepository>();
builder.Services.AddScoped<ITransformer<CustomerOrderFlat>, CustomerOrderFlatTransformer>();
// builder.Services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatDbExtractor>();
builder.Services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatRestApiExtractor>();
builder.Services.AddScoped<ILoader<CustomerOrderFlat>, CustomerOrderFlatSqlBulkCopyLoader>();

builder.Services.AddHostedService<InsertCustomerOrderFlatWorker>();

var host = builder.Build();
await host.RunAsync();