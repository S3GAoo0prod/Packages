using System.Security.Claims;
using Geneirodan.Abstractions.Domain;
using Microsoft.AspNetCore.Http;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Implements the <see cref="IUser"/> interface using information from the current HTTP context.
/// </summary>
public sealed class HttpUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    /// <inheritdoc />
    public bool IsInRole(string role) => httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;

    /// <inheritdoc />
    public Guid Id
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value.AsSpan(), out var result) ? result : Guid.Empty;
        }
    }
}

