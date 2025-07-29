# Drop Database:
dotnet ef database drop --project Sources\Hosts\EtlSandbox.BetaWorkerService\

# Remove Migrations
dotnet ef migrations remove --project Sources\Hosts\EtlSandbox.BetaWorkerService\

# Add Migration
dotnet ef migrations add Initial --project Sources\Hosts\EtlSandbox.BetaWorkerService\

# Apply Migrations
dotnet ef database update --project Sources\Hosts\EtlSandbox.BetaWorkerService\