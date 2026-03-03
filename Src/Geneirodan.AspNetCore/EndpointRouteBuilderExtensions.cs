using System.Diagnostics.CodeAnalysis;
using HealthChecks.UI.Client;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Extension methods for <see cref="IEndpointRouteBuilder"/> to map health-check endpoints with a JSON response writer
/// (HealthChecks UI format). Use these when registering health checks so that the response is machine-readable and consistent.
/// </summary>
[PublicAPI]
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a health check endpoint that returns a JSON response using the HealthChecks UI writer (no exception details in the response).
    /// Call this after <c>AddHealthChecks</c> and any health check registrations. The default route is <c>/health</c>.
    /// </summary>
    /// <param name="routeBuilder">The endpoint route builder (e.g. <c>app</c> in minimal APIs).</param>
    /// <param name="pattern">The route pattern for the health endpoint. Defaults to <c>/health</c>.</param>
    /// <param name="options">Optional health check options; if not provided, only the response writer is set.</param>
    /// <returns>An <see cref="IEndpointConventionBuilder"/> for further configuration.</returns>
    public static IEndpointConventionBuilder MapHealthChecksWithJsonSupport(
        this IEndpointRouteBuilder routeBuilder,
        [StringSyntax("Route"), RouteTemplate] string pattern = "/health",
        HealthCheckOptions? options = null
    )
    {
        options ??= new HealthCheckOptions();
        options.ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponseNoExceptionDetails;
        return routeBuilder.MapHealthChecks(pattern, options);
    }
}