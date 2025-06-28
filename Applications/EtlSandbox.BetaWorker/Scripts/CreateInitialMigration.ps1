# Drop Database:
dotnet ef database drop --project Sources\EtlSandbox.BetaWorker\

# Remove Migrations
dotnet ef migrations remove --project Sources\EtlSandbox.BetaWorker\

# Add Migration
dotnet ef migrations add Initial --project Sources\EtlSandbox.BetaWorker\

# Apply Migrations
dotnet ef database update --project Sources\EtlSandbox.BetaWorker\