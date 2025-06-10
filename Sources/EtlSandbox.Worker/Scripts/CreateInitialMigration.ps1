# Drop Database:
dotnet ef database drop --project Sources\EtlSandbox.Worker\

# Remove Migrations
dotnet ef migrations remove --project Sources\EtlSandbox.Worker\

# Add Migration
dotnet ef migrations add Initial --project Sources\EtlSandbox.Worker\

# Apply Migrations
dotnet ef database update --project Sources\EtlSandbox.Worker\