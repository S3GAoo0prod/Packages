namespace Geneirodan.Abstractions.Domain;

/// <summary>
/// Represents the current user in the system with identifier and role-based access.
/// Implemented in the API layer (e.g. <c>HttpUser</c> in Geneirodan.AspNetCore) using the current HTTP context (claims); used by
/// MediatR authorization and handlers to obtain the acting user without depending on ASP.NET Core types.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Unique identifier of the user.
    /// Sourced from the authentication provider (e.g. JWT <c>NameIdentifier</c> claim). Returns <see cref="Guid.Empty"/>
    /// when the user is not authenticated (e.g. anonymous requests).
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Determines whether the current user is in the specified role.
    /// Used by authorization policies and <c>[Authorize]</c> / <c>[Authorize(Roles = "...")]</c> to enforce role-based access.
    /// </summary>
    /// <param name="role">The name of the role to check against the user's roles (e.g. "Admin", "User").</param>
    /// <returns><see langword="true"/> if the user is in the specified role; otherwise, <see langword="false"/>.</returns>
    bool IsInRole(string role);
}
