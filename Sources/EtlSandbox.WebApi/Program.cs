using EtlSandbox.Domain.CustomerOrderFlats;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;
using EtlSandbox.Shared;
using EtlSandbox.Shared.ConfigureOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSharedConfiguration();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.ConfigureOptions<DatabaseConnectionsSetup>();
builder.Services.AddScoped<IExtractor<CustomerOrderFlat>, CustomerOrderFlatDapperExtractor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// app.UseAuthorization();

app.MapControllers();

app.Run();
