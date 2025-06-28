# Drop Database:
dotnet ef database drop --project Sources\EtlSandbox.AlphaWorker\

# Remove Migrations
dotnet ef migrations remove --project Sources\EtlSandbox.AlphaWorker\

# Add Migration
dotnet ef migrations add Initial --project Sources\EtlSandbox.AlphaWorker\

# Apply Migrations
dotnet ef database update --project Sources\EtlSandbox.AlphaWorker\