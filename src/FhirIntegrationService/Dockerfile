# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj and restore dependencies
COPY ["FhirIntegrationService.csproj", "."]
RUN dotnet restore "FhirIntegrationService.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "FhirIntegrationService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FhirIntegrationService.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Create non-root user for security
RUN groupadd -r fhirapp && useradd --no-log-init -r -g fhirapp fhirapp

WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory and set permissions
RUN mkdir -p /app/logs && \
    chown -R fhirapp:fhirapp /app && \
    chmod 755 /app

# Switch to non-root user
USER fhirapp

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Configure health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "FhirIntegrationService.dll"]