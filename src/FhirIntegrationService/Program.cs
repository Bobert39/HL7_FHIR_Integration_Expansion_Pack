using FhirIntegrationService.Authorization;
using FhirIntegrationService.Configuration;
using FhirIntegrationService.Middleware;
using FhirIntegrationService.Services;
using FhirIntegrationService.Services.Interfaces;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Filter.ByExcluding(logEvent =>
        logEvent.MessageTemplate.Text.Contains("patient", StringComparison.OrdinalIgnoreCase) ||
        logEvent.MessageTemplate.Text.Contains("ssn", StringComparison.OrdinalIgnoreCase) ||
        logEvent.MessageTemplate.Text.Contains("birthdate", StringComparison.OrdinalIgnoreCase))
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configure FHIR Services
builder.Services.Configure<FhirConfiguration>(
    builder.Configuration.GetSection("Fhir"));

// Configure Data Mapping Services
builder.Services.Configure<DataMappingConfiguration>(
    builder.Configuration.GetSection(DataMappingConfiguration.SectionName));

// Configure Vendor API Client
builder.Services.Configure<VendorApiConfiguration>(
    builder.Configuration.GetSection("VendorApi"));

// Configure SMART Authentication
builder.Services.Configure<SmartAuthConfiguration>(
    builder.Configuration.GetSection("SmartAuth"));

// Configure FHIR Validation
builder.Services.Configure<FhirValidationConfiguration>(
    builder.Configuration.GetSection("FhirValidation"));

builder.Services.AddScoped<FhirClient>(provider =>
{
    var config = builder.Configuration.GetSection("Fhir").Get<FhirConfiguration>();
    return new FhirClient(config?.ServerUrl ?? "https://hapi.fhir.org/baseR4");
});

// Register FHIR JSON Serializer
builder.Services.AddSingleton<FhirJsonSerializer>();

// Register FHIR Resource Resolver
builder.Services.AddSingleton<IResourceResolver>(provider =>
{
    return new CachedResolver(ZipSource.CreateValidationSource());
});

// Configure HTTP Client with Polly resilience policies
builder.Services.AddHttpClient<IVendorApiClient, VendorApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

// Configure HTTP Client for SMART Authentication
builder.Services.AddHttpClient<ISmartAuthenticationService, SmartAuthenticationService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Register services
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();
builder.Services.AddScoped<IDataMappingService, DataMappingService>();
builder.Services.AddScoped<IVendorApiClient, VendorApiClient>();
builder.Services.AddScoped<IFhirValidationService, FhirValidationService>();
builder.Services.AddScoped<ISmartAuthenticationService, SmartAuthenticationService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, SmartScopeAuthorizationHandler>();

// Add authorization with SMART on FHIR policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(SmartAuthorizationPolicies.PatientRead, policy =>
        policy.Requirements.Add(new SmartScopeRequirement("patient/*.read")));
    options.AddPolicy(SmartAuthorizationPolicies.PatientWrite, policy =>
        policy.Requirements.Add(new SmartScopeRequirement("patient/*.write")));
    options.AddPolicy(SmartAuthorizationPolicies.PatientAll, policy =>
        policy.Requirements.Add(new SmartScopeRequirement("patient/*.*")));
    options.AddPolicy(SmartAuthorizationPolicies.ObservationRead, policy =>
        policy.Requirements.Add(new SmartScopeRequirement("observation/*.read")));
});

// Configure health checks
builder.Services.AddHealthChecks()
    .AddCheck("fhir_server", () => HealthCheckResult.Healthy("FHIR server connection available"))
    .AddCheck("vendor_api", () => HealthCheckResult.Healthy("Vendor API connection available"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FHIR Integration Service",
        Version = "v1",
        Description = "A service for integrating with FHIR-compliant healthcare systems"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add SMART authentication middleware
app.UseMiddleware<SmartAuthenticationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Polly policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Log.Warning("Retry {RetryCount} for {RequestUri} in {Delay}ms",
                    retryCount, context.GetValueOrDefault("RequestUri"), timespan.TotalMilliseconds);
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            samplingDuration: TimeSpan.FromSeconds(60),
            minimumThroughput: 5,
            failureThreshold: 0.5,
            breakDuration: TimeSpan.FromSeconds(60),
            onBreak: (exception, duration) =>
            {
                Log.Warning("Circuit breaker opened for {Duration}ms", duration.TotalMilliseconds);
            },
            onReset: () =>
            {
                Log.Information("Circuit breaker reset");
            });
}