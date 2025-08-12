using EtlSandbox.AlphaWorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfigureOptions();
builder.Services.AddLogs();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var host = builder.Build();
await host.RunAsync();