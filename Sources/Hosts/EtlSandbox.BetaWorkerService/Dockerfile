FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["EtlSandbox.sln", "."]
COPY ["Sources/Hosts/EtlSandbox.AlphaWorkerService/EtlSandbox.AlphaWorkerService.csproj", "Sources/Hosts/EtlSandbox.AlphaWorkerService/"]
COPY ["Sources/Hosts/EtlSandbox.DeltaWorkerService/EtlSandbox.DeltaWorkerService.csproj", "Sources/Hosts/EtlSandbox.DeltaWorkerService/"]
COPY ["Sources/Hosts/EtlSandbox.BetaWorkerService/EtlSandbox.BetaWorkerService.csproj", "Sources/Hosts/EtlSandbox.BetaWorkerService/"]
COPY ["Sources/Hosts/EtlSandbox.BetaWebApi/EtlSandbox.BetaWebApi.csproj", "Sources/Hosts/EtlSandbox.BetaWebApi/"]
COPY ["Sources/Hosts/EtlSandbox.GammaWorkerService/EtlSandbox.GammaWorkerService.csproj", "Sources/Hosts/EtlSandbox.GammaWorkerService/"]
COPY ["Sources/Hosts/EtlSandbox.DeltaWebApi/EtlSandbox.DeltaWebApi.csproj", "Sources/Hosts/EtlSandbox.DeltaWebApi/"]
COPY ["Sources/Infrastructures/EtlSandbox.Infrastructure/EtlSandbox.Infrastructure.csproj", "Sources/Infrastructures/EtlSandbox.Infrastructure/"]
COPY ["Sources/Presentations/EtlSandbox.Presentation/EtlSandbox.Presentation.csproj", "Sources/Presentations/EtlSandbox.Presentation/"]
COPY ["Sources/Cores/EtlSandbox.Application/EtlSandbox.Application.csproj", "Sources/Cores/EtlSandbox.Application/"]
COPY ["Sources/Cores/EtlSandbox.Domain/EtlSandbox.Domain.csproj", "Sources/Cores/EtlSandbox.Domain/"]

# Restore dependencies
RUN dotnet restore "EtlSandbox.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/Sources/Hosts/EtlSandbox.BetaWorkerService"
RUN dotnet build "EtlSandbox.BetaWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EtlSandbox.BetaWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtlSandbox.BetaWorkerService.dll"]
