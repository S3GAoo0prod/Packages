using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;

namespace Geneirodan.Abstractions.Domain;

/// <summary>
/// Represents an abstract base class for domain entities that implement <see cref="IEntity{TKey}"/>.
/// Inherit from this class when defining domain entities so that they share a common identifier contract
/// and can be used with <see cref="IRepository{TEntity,TKey}"/> and Entity Framework Core mappings.
/// </summary>
/// <typeparam name="TKey">
/// The type of the identifier. It must implement <see cref="IEquatable{TKey}"/> for repository and EF Core support.
/// </typeparam>
[PublicAPI]
public abstract class Entity<TKey> : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Unique identifier of the entity.
    /// Required and init-only; typically set by the persistence layer (e.g. database or in-memory store) on insert.
    /// </summary>
    public required TKey Id { get; init; }
}
