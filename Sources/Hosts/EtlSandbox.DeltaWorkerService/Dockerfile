FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sources/Hosts/EtlSandbox.DeltaWorkerService/EtlSandbox.DeltaWorkerService.csproj", "Sources/Hosts/EtlSandbox.DeltaWorkerService/"]
COPY ["Sources/Presentations/EtlSandbox.Presentation/EtlSandbox.Presentation.csproj", "Sources/Presentations/EtlSandbox.Presentation/"]
COPY ["Sources/Cores/EtlSandbox.Application/EtlSandbox.Application.csproj", "Sources/Cores/EtlSandbox.Application/"]
COPY ["Sources/Cores/EtlSandbox.Domain/EtlSandbox.Domain.csproj", "Sources/Cores/EtlSandbox.Domain/"]
COPY ["Sources/Infrastructures/EtlSandbox.Infrastructure/EtlSandbox.Infrastructure.csproj", "Sources/Infrastructures/EtlSandbox.Infrastructure/"]
RUN dotnet restore "Sources/Hosts/EtlSandbox.DeltaWorkerService/EtlSandbox.DeltaWorkerService.csproj"
COPY . .
WORKDIR "/src/Sources/Hosts/EtlSandbox.DeltaWorkerService"
RUN dotnet build "./EtlSandbox.DeltaWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EtlSandbox.DeltaWorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtlSandbox.DeltaWorkerService.dll"]
