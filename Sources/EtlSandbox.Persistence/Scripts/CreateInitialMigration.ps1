# Drop Database:
dotnet ef database drop --project Sources\EtlSandbox.Persistence\

# Remove Migrations
dotnet ef migrations remove --project Sources\EtlSandbox.Persistence\

# Add Migration
dotnet ef migrations add Initial --project Sources\EtlSandbox.Persistence\

# Apply Migrations
dotnet ef database update --project Sources\EtlSandbox.Persistence\