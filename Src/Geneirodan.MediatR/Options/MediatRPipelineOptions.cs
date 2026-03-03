using Geneirodan.MediatR.Attributes;
using Geneirodan.MediatR.Behaviors;
using JetBrains.Annotations;

namespace Geneirodan.MediatR.Options;

/// <summary>
/// Options passed to <c>DependencyInjection.AddMediatRPipeline</c> to enable or disable each pipeline behavior.
/// Order of execution when enabled: logging (pre-processor), then authorization, validation, unhandled exception handling, then handler.
/// </summary>
[PublicAPI]
public sealed record MediatRPipelineOptions
{
    /// <summary>
    /// When <see langword="true"/>, <see cref="AuthorizationBehavior{TRequest,TResponse}"/> runs before the handler and enforces
    /// <see cref="AuthorizeAttribute"/> and role requirements using <see cref="Geneirodan.Abstractions.Domain.IUser"/>.
    /// </summary>
    public bool UseAuthorization { get; init; } = true;

    /// <summary>
    /// When <see langword="true"/>, <see cref="LoggingPreProcessor{TRequest}"/> runs first and logs the request (and user ID when available).
    /// </summary>
    public bool UseLogging { get; init; } = true;

    /// <summary>
    /// When <see langword="true"/>, <see cref="UnhandledExceptionBehavior{TRequest,TResponse}"/> wraps the handler and logs any exception before rethrowing.
    /// </summary>
    public bool UseExceptions { get; init; } = true;

    /// <summary>
    /// When <see langword="true"/>, <see cref="ValidationBehavior{TRequest,TResponse}"/> runs FluentValidation validators for the request
    /// and returns a validation result without calling the handler if validation fails.
    /// </summary>
    public bool UseValidation { get; init; } = true;
}