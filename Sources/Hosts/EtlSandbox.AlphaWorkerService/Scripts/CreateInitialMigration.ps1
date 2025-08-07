# Drop Database:
dotnet ef database drop --project Sources\Infrastructures\EtlSandbox.Infrastructure.Mars --startup-project Sources\Hosts\EtlSandbox.AlphaWorkerService --context MarsDbContext

# Remove Migrations
dotnet ef migrations remove --project Sources\Infrastructures\EtlSandbox.Infrastructure.Mars --startup-project Sources\Hosts\EtlSandbox.AlphaWorkerService --context MarsDbContext

# Add Migration
dotnet ef migrations add Initial --project Sources\Infrastructures\EtlSandbox.Infrastructure.Mars --startup-project Sources\Hosts\EtlSandbox.AlphaWorkerService --context MarsDbContext

# Apply Migrations
dotnet ef database update --project Sources\Infrastructures\EtlSandbox.Infrastructure.Mars --startup-project Sources\Hosts\EtlSandbox.AlphaWorkerService --context MarsDbContext