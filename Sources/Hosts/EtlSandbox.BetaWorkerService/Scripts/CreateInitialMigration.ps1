# Drop Database:
dotnet ef database drop --project Sources\Infrastructures\EtlSandbox.Infrastructure.Neptune --startup-project Sources\Hosts\EtlSandbox.BetaWorkerService --context NeptuneDbContext

# Remove Migrations
dotnet ef migrations remove --project Sources\Infrastructures\EtlSandbox.Infrastructure.Neptune --startup-project Sources\Hosts\EtlSandbox.BetaWorkerService --context NeptuneDbContext

# Add Migration
dotnet ef migrations add Initial --project Sources\Infrastructures\EtlSandbox.Infrastructure.Neptune --startup-project Sources\Hosts\EtlSandbox.BetaWorkerService --context NeptuneDbContext

# Apply Migrations
dotnet ef database update --project Sources\Infrastructures\EtlSandbox.Infrastructure.Neptune --startup-project Sources\Hosts\EtlSandbox.BetaWorkerService --context NeptuneDbContext