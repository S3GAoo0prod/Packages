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
/// A static class for registering and configuring observability-related services, such as OpenTelemetry and Serilog.
/// </summary>
[PublicAPI]
public static class DependencyInjection
{
    private const string OtelEndpointName = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary>
    /// Adds OpenTelemetry services to the provided <see cref="IServiceCollection"/> for collecting metrics and tracing.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register services with.</param>
    /// <param name="configuration">The configuration instance containing the OpenTelemetry settings.</param>
    /// <param name="sectionName">The section name in the configuration to bind OpenTelemetry settings. Defaults to "OpenTelemetry".</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with OpenTelemetry services added.</returns>

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
    /// Configures Serilog logging for the application, including OpenTelemetry integration.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to configure.</param>
    /// <returns>The updated <see cref="IHostApplicationBuilder"/> with Serilog configured.</returns>
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