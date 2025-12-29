# ===================================
# Stage 1: Build
# ===================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["ZetaFin.API/ZetaFin.API.csproj", "ZetaFin.API/"]
COPY ["ZetaFin.Application/ZetaFin.Application.csproj", "ZetaFin.Application/"]
COPY ["ZetaFin.Domain/ZetaFin.Domain.csproj", "ZetaFin.Domain/"]
COPY ["ZetaFin.Infrastructure/ZetaFin.Infrastructure.csproj", "ZetaFin.Infrastructure/"]
COPY ["ZetaFin.Persistence/ZetaFin.Persistence.csproj", "ZetaFin.Persistence/"]

RUN dotnet restore "ZetaFin.API/ZetaFin.API.csproj"

# Copiar todo o código e compilar
COPY . .
WORKDIR "/src/ZetaFin.API"
RUN dotnet build "ZetaFin.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===================================
# Stage 2: Publish
# ===================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ZetaFin.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ===================================
# Stage 3: Runtime (Final)
# ===================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Configurar usuário não-root para segurança
RUN addgroup --system --gid 1001 zetafin && \
    adduser --system --uid 1001 --ingroup zetafin zetafin

WORKDIR /app

# Instalar ferramentas necessárias
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

# Copiar aplicação publicada
COPY --from=publish --chown=zetafin:zetafin /app/publish .

# Copiar script de inicialização
COPY --chown=zetafin:zetafin ZetaFin.API/docker-entrypoint.sh /app/
RUN chmod +x /app/docker-entrypoint.sh

# Mudar para usuário não-root
USER zetafin

# Expor portas
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Usar script de entrada
ENTRYPOINT ["/app/docker-entrypoint.sh"]