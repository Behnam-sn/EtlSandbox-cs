# Drop Database:
dotnet ef database drop --project Sources\Hosts\EtlSandbox.AlphaWorkerService\

# Remove Migrations
dotnet ef migrations remove --project Sources\Hosts\EtlSandbox.AlphaWorkerService\

# Add Migration
dotnet ef migrations add Initial --project Sources\Hosts\EtlSandbox.AlphaWorkerService\

# Apply Migrations
dotnet ef database update --project Sources\Hosts\EtlSandbox.AlphaWorkerService\