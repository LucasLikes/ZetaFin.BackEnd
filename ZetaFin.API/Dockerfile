# Base image para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar csproj existentes
COPY ["ZetaFin.API/ZetaFin.API.csproj", "ZetaFin.API/"]
COPY ["ZetaFin.Application/ZetaFin.Application.csproj", "ZetaFin.Application/"]
COPY ["ZetaFin.Persistence/ZetaFin.Persistence.csproj", "ZetaFin.Persistence/"]
COPY ["ZetaFin.Domain/ZetaFin.Domain.csproj", "ZetaFin.Domain/"]

# Restore
RUN dotnet restore "ZetaFin.API/ZetaFin.API.csproj"

# Copiar todo o código
COPY . .

WORKDIR "/src/ZetaFin.API"

# Build
RUN dotnet build "ZetaFin.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ZetaFin.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZetaFin.API.dll"]
