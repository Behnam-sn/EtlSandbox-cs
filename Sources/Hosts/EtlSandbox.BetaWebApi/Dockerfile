FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sources/Hosts/EtlSandbox.BetaWebApi/EtlSandbox.BetaWebApi.csproj", "Sources/Hosts/EtlSandbox.BetaWebApi/"]
COPY ["Sources/Presentations/EtlSandbox.Presentation/EtlSandbox.Presentation.csproj", "Sources/Presentations/EtlSandbox.Presentation/"]
COPY ["Sources/Cores/EtlSandbox.Application/EtlSandbox.Application.csproj", "Sources/Cores/EtlSandbox.Application/"]
COPY ["Sources/Cores/EtlSandbox.Domain/EtlSandbox.Domain.csproj", "Sources/Cores/EtlSandbox.Domain/"]
COPY ["Sources/Infrastructures/EtlSandbox.Infrastructure/EtlSandbox.Infrastructure.csproj", "Sources/Infrastructures/EtlSandbox.Infrastructure/"]
RUN dotnet restore "Sources/Hosts/EtlSandbox.BetaWebApi/EtlSandbox.BetaWebApi.csproj"
COPY . .
WORKDIR "/src/Sources/Hosts/EtlSandbox.BetaWebApi"
RUN dotnet build "./EtlSandbox.BetaWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EtlSandbox.BetaWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtlSandbox.BetaWebApi.dll"]
