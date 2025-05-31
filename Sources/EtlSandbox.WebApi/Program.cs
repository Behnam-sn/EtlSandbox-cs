using EtlSandbox.Infrastructure;
using EtlSandbox.Shared.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.ConfigureOptions<ConnectionStringsSetup>();
builder.Services.AddScoped<CustomerOrderFlatService>();


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
