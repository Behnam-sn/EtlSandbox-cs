# Drop Database:
dotnet ef database drop --project Applications\EtlSandbox.AlphaWorker\

# Remove Migrations
dotnet ef migrations remove --project Applications\EtlSandbox.AlphaWorker\

# Add Migration
dotnet ef migrations add Initial --project Applications\EtlSandbox.AlphaWorker\

# Apply Migrations
dotnet ef database update --project Applications\EtlSandbox.AlphaWorker\