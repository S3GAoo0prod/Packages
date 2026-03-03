using JetBrains.Annotations;

namespace Geneirodan.Observability;

/// <summary>
/// Configuration for OpenTelemetry metrics and tracing. Bound from the <c>OpenTelemetry</c> config section when
/// <see cref="DependencyInjection.AddSharedOpenTelemetry"/> is used. When a section is <see langword="null"/>, that signal is not registered.
/// </summary>
[PublicAPI]
public sealed record OpenTelemetrySettings
{
    /// <summary>
    /// When set, configures which metrics instrumentation and meters are enabled and exported via OTLP.
    /// </summary>
    public MetricsSettings? Metrics { get; init; }

    /// <summary>
    /// When set, configures which tracing instrumentation is enabled (AspNetCore, HttpClient, EF Core) and exports spans via OTLP.
    /// </summary>
    public TracingSettings? Tracing { get; init; }

    /// <summary>
    /// Options for OpenTelemetry metrics: which instrumentations to enable and which custom meters to add.
    /// </summary>
    public sealed record MetricsSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether runtime instrumentation is enabled.
        /// </summary>
        public bool UseRuntimeInstrumentation { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether ASP.NET Core instrumentation is enabled.
        /// </summary>
        public bool UseAspNetCoreInstrumentation { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP client instrumentation is enabled.
        /// </summary>
        public bool UseHttpClientInstrumentation { get; init; }

        /// <summary>
        /// Gets or sets an array of meters to track for metrics collection.
        /// </summary>
        public required string[] Meters { get; init; }
    }

    /// <summary>
    /// Options for OpenTelemetry tracing: which instrumentations (AspNetCore, HTTP client, EF Core) to enable.
    /// </summary>
    public sealed record TracingSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether ASP.NET Core tracing instrumentation is enabled.
        /// </summary>
        public bool UseAspNetCoreInstrumentation { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTP client tracing instrumentation is enabled.
        /// </summary>
        public bool UseHttpClientInstrumentation { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether Entity Framework Core tracing instrumentation is enabled.
        /// </summary>
        public bool UseEntityFrameworkCoreInstrumentation { get; init; }
    }
}
