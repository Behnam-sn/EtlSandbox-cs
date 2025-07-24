using EtlSandbox.BetaWebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigureOptions();
builder.Services.AddLogs();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();