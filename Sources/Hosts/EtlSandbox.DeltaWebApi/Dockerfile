FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sources/Hosts/EtlSandbox.DeltaWebApi/EtlSandbox.DeltaWebApi.csproj", "Sources/Hosts/EtlSandbox.DeltaWebApi/"]
COPY ["Sources/Infrastructures/EtlSandbox.Infrastructure/EtlSandbox.Infrastructure.csproj", "Sources/Infrastructures/EtlSandbox.Infrastructure/"]
COPY ["Sources/Cores/EtlSandbox.Application/EtlSandbox.Application.csproj", "Sources/Cores/EtlSandbox.Application/"]
COPY ["Sources/Cores/EtlSandbox.Domain/EtlSandbox.Domain.csproj", "Sources/Cores/EtlSandbox.Domain/"]
COPY ["Sources/Presentations/EtlSandbox.Presentation/EtlSandbox.Presentation.csproj", "Sources/Presentations/EtlSandbox.Presentation/"]
RUN dotnet restore "Sources/Hosts/EtlSandbox.DeltaWebApi/EtlSandbox.DeltaWebApi.csproj"
COPY . .
WORKDIR "/src/Sources/Hosts/EtlSandbox.DeltaWebApi"
RUN dotnet build "./EtlSandbox.DeltaWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EtlSandbox.DeltaWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtlSandbox.DeltaWebApi.dll"]
