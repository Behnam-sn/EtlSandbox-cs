# Drop Database:
dotnet ef database drop --project Sources\Hosts\EtlSandbox.GammaWorkerService\

# Remove Migrations
dotnet ef migrations remove --project Sources\Hosts\EtlSandbox.GammaWorkerService\

# Add Migration
dotnet ef migrations add Initial --project Sources\Hosts\EtlSandbox.GammaWorkerService\

# Apply Migrations
dotnet ef database update --project Sources\Hosts\EtlSandbox.GammaWorkerService\