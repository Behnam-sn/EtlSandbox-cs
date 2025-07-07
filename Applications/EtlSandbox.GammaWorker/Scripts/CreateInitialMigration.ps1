# Drop Database:
dotnet ef database drop --project Applications\EtlSandbox.GammaWorker\

# Remove Migrations
dotnet ef migrations remove --project Applications\EtlSandbox.GammaWorker\

# Add Migration
dotnet ef migrations add Initial --project Applications\EtlSandbox.GammaWorker\

# Apply Migrations
dotnet ef database update --project Applications\EtlSandbox.GammaWorker\