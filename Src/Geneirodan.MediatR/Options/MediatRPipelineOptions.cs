using JetBrains.Annotations;

namespace Geneirodan.MediatR.Options;

/// <summary>
/// Represents configuration options for controlling which pipeline behaviors are enabled or disabled
/// in the MediatR request processing pipeline.
/// </summary>
[PublicAPI]
public sealed record MediatRPipelineOptions
{
    /// <summary>
    /// Indicates whether the authorization behavior should be applied in the MediatR pipeline.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool UseAuthorization { get; init; } = true;

    /// <summary>
    /// Indicates whether the logging behavior should be applied in the MediatR pipeline.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool UseLogging { get; init; } = true;

    /// <summary>
    /// Indicates whether the unhandled exception handling behavior should be applied in the MediatR pipeline.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool UseExceptions { get; init; } = true;

    /// <summary>
    /// Indicates whether the validation behavior should be applied in the MediatR pipeline.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool UseValidation { get; init; } = true;
}