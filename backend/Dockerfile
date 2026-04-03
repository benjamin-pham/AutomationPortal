# syntax=docker/dockerfile:1

# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Clear any Windows-specific NuGet fallback folders that may be baked into the SDK image
ENV NUGET_FALLBACK_PACKAGES=""

# Copy solution-level props first for layer caching
COPY Directory.Build.props Directory.Packages.props ./

# Copy project files and restore (layer-cached until .csproj changes)
COPY src/AutomationPortal.Domain/AutomationPortal.Domain.csproj              src/AutomationPortal.Domain/
COPY src/AutomationPortal.Application/AutomationPortal.Application.csproj    src/AutomationPortal.Application/
COPY src/AutomationPortal.Infrastructure/AutomationPortal.Infrastructure.csproj src/AutomationPortal.Infrastructure/
COPY src/AutomationPortal.API/AutomationPortal.API.csproj                    src/AutomationPortal.API/

RUN dotnet restore src/AutomationPortal.API/AutomationPortal.API.csproj

# Copy remaining source and publish
COPY src/ src/
RUN dotnet publish src/AutomationPortal.API/AutomationPortal.API.csproj \
    -c Release \
    -o /app/publish

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd --system --gid 1001 dotnet \
 && useradd  --system --uid 1001 --gid dotnet --no-create-home dotnet

COPY --from=build --chown=dotnet:dotnet /app/publish ./

# Ensure log directory exists
RUN mkdir -p logs && chown dotnet:dotnet logs

USER dotnet

EXPOSE 5000

ENTRYPOINT ["dotnet", "AutomationPortal.API.dll"]
