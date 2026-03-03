using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using static Serilog.Sinks.OpenTelemetry.IncludedData;


namespace Geneirodan.Observability;

/// <summary>
/// Extension methods to register OpenTelemetry (metrics and tracing) and Serilog with OTLP export.
/// When the OTLP endpoint is not configured (<c>OTEL_EXPORTER_OTLP_ENDPOINT</c>), OpenTelemetry registration is skipped; Serilog still configures console and any sinks from configuration.
/// </summary>
[PublicAPI]
public static class DependencyInjection
{
    private const string OtelEndpointName = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary>
    /// Registers OpenTelemetry metrics and tracing with OTLP exporter if <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is set in configuration.
    /// Binds <see cref="OpenTelemetrySettings"/> from the specified config section and enables instrumentation (AspNetCore, Http, EF Core, runtime) and meters as configured.
    /// Service name is taken from configuration or the entry assembly name.
    /// </summary>
    /// <param name="services">The service collection to register OpenTelemetry with.</param>
    /// <param name="configuration">The configuration that contains the OTLP endpoint and the OpenTelemetry section.</param>
    /// <param name="sectionName">The configuration section name for <see cref="OpenTelemetrySettings"/>. Defaults to <c>OpenTelemetry</c>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddSharedOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "OpenTelemetry"
    )
    {
        if (string.IsNullOrWhiteSpace(configuration[OtelEndpointName]))
            return services;

        var settings = services
            .AddOptions<OpenTelemetrySettings>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<OpenTelemetrySettings>>()
            .Value;

        var builder = services.AddOpenTelemetry().ConfigureResource(x => x.AddService(configuration.GetServiceName()));

        if (settings.Metrics is { } metrics)
            builder.WithMetrics(x =>
            {
                if (metrics.UseRuntimeInstrumentation)
                    x.AddRuntimeInstrumentation();
                if (metrics.UseAspNetCoreInstrumentation)
                    x.AddAspNetCoreInstrumentation();
                if (metrics.UseHttpClientInstrumentation)
                    x.AddHttpClientInstrumentation();
                if (metrics.Meters.Length > 0)
                    x.AddMeter(metrics.Meters);
                x.AddOtlpExporter();
            });

        if (settings.Tracing is { } tracing)
            builder.WithTracing(x =>
            {
                if (tracing.UseAspNetCoreInstrumentation)
                    x.AddAspNetCoreInstrumentation();
                if (tracing.UseHttpClientInstrumentation)
                    x.AddHttpClientInstrumentation();
                if (tracing.UseEntityFrameworkCoreInstrumentation)
                    x.AddEntityFrameworkCoreInstrumentation();
                x.AddOtlpExporter();
            });

        return services;
    }

    /// <summary>
    /// Configures Serilog to read from host configuration and services, enriches with log context and application name,
    /// writes to console and (when <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is set) to OpenTelemetry via OTLP gRPC with trace/span context.
    /// Call this on the host builder before building so that startup and pipeline use Serilog.
    /// </summary>
    /// <param name="builder">The host application builder (e.g. <c>WebApplication.CreateBuilder(args)</c>).</param>
    /// <returns>The updated <see cref="IHostApplicationBuilder"/>.</returns>
    public static IHostApplicationBuilder AddSerilog(this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Configuration.GetServiceName();
        builder.Services.AddSerilog((sp, serilog) =>
            serilog
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(sp)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", serviceName)
                .WriteTo.Console()
                .WriteTo.OpenTelemetry(c =>
                    {
                        c.Endpoint = builder.Configuration[OtelEndpointName];
                        c.Protocol = OtlpProtocol.Grpc;
                        c.IncludedData = TraceIdField | SpanIdField | SourceContextAttribute;
                        c.ResourceAttributes = new Dictionary<string, object>(StringComparer.Ordinal)
                        {
                            { "service.name", serviceName }
                        };
                    }
                ));

        return builder;
    }

    private static string GetServiceName(this IConfiguration configuration) =>
        configuration["ServiceName"] ?? Assembly.GetEntryAssembly()!.GetName().Name!;
}