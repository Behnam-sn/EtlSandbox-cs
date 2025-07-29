using EtlSandbox.BetaWorkerService;
using EtlSandbox.Infrastructure.Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddSharedConfiguration();

builder.Services.AddConfigureOptions();
builder.Services.AddLogs();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var host = builder.Build();
host.Run();