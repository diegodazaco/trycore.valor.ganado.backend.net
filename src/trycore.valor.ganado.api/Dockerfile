# ── Etapa 1: compilación ─────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solución y proyectos para aprovechar la caché de capas en restore
COPY trycore.valor.ganado.backend.net.sln ./
COPY src/ src/

RUN dotnet restore src/trycore.valor.ganado.api/trycore.valor.ganado.api.csproj

# Publicar en modo Release
RUN dotnet publish src/trycore.valor.ganado.api/trycore.valor.ganado.api.csproj \
        -c Release \
        -o /publish \
        --no-restore

# ── Etapa 2: imagen de ejecución ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "trycore.valor.ganado.api.dll"]
