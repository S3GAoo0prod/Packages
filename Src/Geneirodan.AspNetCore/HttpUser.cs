using System.Security.Claims;
using Geneirodan.Abstractions.Domain;
using Microsoft.AspNetCore.Http;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Implements <see cref="IUser"/> by reading the current HTTP context (claims principal).
/// Allows handlers and authorization to depend on <see cref="IUser"/> instead of <see cref="HttpContext"/>.
/// </summary>
public sealed class HttpUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    /// <summary>
    /// Checks whether the current request's user is in the given role, using the authentication scheme's role claims.
    /// Returns <see langword="false"/> when there is no HTTP context or no authenticated user.
    /// </summary>
    /// <param name="role">The role name to check (e.g. "Admin").</param>
    /// <returns><see langword="true"/> if the user is in the specified role; otherwise <see langword="false"/>.</returns>
    public bool IsInRole(string role) => httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;

    /// <summary>
    /// Gets the user identifier from the <c>NameIdentifier</c> claim. Returns <see cref="Guid.Empty"/> when the user
    /// is not authenticated or the claim is missing or invalid.
    /// </summary>
    public Guid Id
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value.AsSpan(), out var result) ? result : Guid.Empty;
        }
    }
}

