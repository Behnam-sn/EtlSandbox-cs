# Drop Database:
dotnet ef database drop --project Sources\Infrastructures\EtlSandbox.Infrastructure.Venus --startup-project Sources\Hosts\EtlSandbox.GammaWorkerService --context VenusDbContext

# Remove Migrations
dotnet ef migrations remove --project Sources\Infrastructures\EtlSandbox.Infrastructure.Venus --startup-project Sources\Hosts\EtlSandbox.GammaWorkerService --context VenusDbContext

# Add Migration
dotnet ef migrations add Initial --project Sources\Infrastructures\EtlSandbox.Infrastructure.Venus --startup-project Sources\Hosts\EtlSandbox.GammaWorkerService --context VenusDbContext

# Apply Migrations
dotnet ef database update --project Sources\Infrastructures\EtlSandbox.Infrastructure.Venus --startup-project Sources\Hosts\EtlSandbox.GammaWorkerService --context VenusDbContext