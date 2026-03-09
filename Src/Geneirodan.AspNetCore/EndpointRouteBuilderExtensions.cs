using System.Diagnostics.CodeAnalysis;
using HealthChecks.UI.Client;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to map health check endpoints
/// with a default or specified route pattern and response writer.
/// </summary>
[PublicAPI]
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a health check endpoint that returns a JSON response.
    /// </summary>
    /// <inheritdoc cref="HealthCheckEndpointRouteBuilderExtensions.MapHealthChecks(IEndpointRouteBuilder, string, HealthCheckOptions?)"/>
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