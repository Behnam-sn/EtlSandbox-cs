using EtlSandbox.Infrastructure.Shared;
using EtlSandbox.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSharedConfiguration();

builder.Services.AddConfigureOptions();
builder.Services.AddLogs();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

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
