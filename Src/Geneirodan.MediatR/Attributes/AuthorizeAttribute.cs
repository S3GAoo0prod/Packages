using Geneirodan.Abstractions.Domain;
using Geneirodan.MediatR.Behaviors;

namespace Geneirodan.MediatR.Attributes;

/// <summary>
/// Applied to a request type (command or query) to enforce authorization in the MediatR pipeline.
/// <see cref="AuthorizationBehavior{TRequest, TResponse}"/> reads this attribute: when present,
/// the current user (<see cref="IUser"/>) must be authenticated; when <see cref="Roles"/> is set, the user must be in at least one of the roles.
/// Can be applied multiple times to the same class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class with no roles (authenticated user only).
    /// </summary>
    public AuthorizeAttribute() { }

    /// <summary>
    /// Comma-delimited list of role names that are allowed to execute the request. If empty, only authentication is required.
    /// The user must be in at least one of these roles (via <see cref="IUser.IsInRole"/>).
    /// </summary>
    public string Roles { get; init; } = string.Empty;
}