namespace Geneirodan.Abstractions.Domain;

/// <summary>
/// Represents a user in the system with basic properties and capabilities, such as the user's identifier and role-based access.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Unique identifier of the user.
    /// The identifier is nullable in case the user is not yet assigned an ID (e.g., for anonymous users).
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Determines whether the user is in the specified role.
    /// </summary>
    /// <param name="role">The name of the role to check against the user's roles.</param>
    /// <returns>True if the user is in the specified role; otherwise, false.</returns>
    bool IsInRole(string role);
}
