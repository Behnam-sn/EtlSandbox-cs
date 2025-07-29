# Drop Database:
dotnet ef database drop --project Applications\EtlSandbox.BetaWorker\

# Remove Migrations
dotnet ef migrations remove --project Applications\EtlSandbox.BetaWorker\

# Add Migration
dotnet ef migrations add Initial --project Applications\EtlSandbox.BetaWorker\

# Apply Migrations
dotnet ef database update --project Applications\EtlSandbox.BetaWorker\