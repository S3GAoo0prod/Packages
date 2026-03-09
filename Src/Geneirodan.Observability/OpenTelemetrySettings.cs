using JetBrains.Annotations;

namespace Geneirodan.Observability;

/// <summary>
/// Represents the settings for OpenTelemetry configuration.
/// </summary>
[PublicAPI]
public sealed record OpenTelemetrySettings
{
    /// <summary>
    /// Gets or sets the metrics-related configuration settings.
    /// 
    /// </summary>
    public MetricsSettings? Metrics { get; init; }

    /// <summary>
    /// Gets or sets the tracing-related configuration settings.
    /// </summary>
    public TracingSettings? Tracing { get; init; }

    /// <summary>
    /// Represents the settings for OpenTelemetry metrics instrumentation.
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
    /// Represents the settings for OpenTelemetry tracing instrumentation.
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
