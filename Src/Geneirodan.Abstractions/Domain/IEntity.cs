using Geneirodan.Abstractions.Repositories;

namespace Geneirodan.Abstractions.Domain;

/// <summary>
/// Defines a contract for an entity that has an identifier of type <typeparamref name="TKey"/>.
/// This interface is used by the domain layer and by <see cref="IRepository{TEntity,TKey}"/> to represent
/// persistable entities with a unique key. All entities exposed through repositories must implement this interface.
/// </summary>
/// <typeparam name="TKey">
/// The type of the identifier for the entity. It must implement <see cref="IEquatable{TKey}"/> to support
/// equality comparisons in repository lookups and to satisfy Entity Framework Core key constraints.
/// </typeparam>
/// <remarks>
/// <seealso cref="Entity{TKey}"/> — base class for domain entities implementing this interface.<br/>
/// <seealso cref="IRepository{TEntity,TKey}"/> — contract that consumes <see cref="IEntity{TKey}"/>.
/// </remarks>
public interface IEntity<out TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Unique identifier of the entity.
    /// Used by the repository layer for lookups, updates, and deletes; must be set before persistence.
    /// </summary>
    public TKey Id { get; }
}
