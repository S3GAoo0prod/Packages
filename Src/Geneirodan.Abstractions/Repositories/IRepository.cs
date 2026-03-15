using Geneirodan.Abstractions.Domain;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Defines the contract for a repository that manages entities of type <typeparamref name="TEntity"/>
/// with a primary key of type <typeparamref name="TKey"/>.
/// Repositories abstract persistence and are used together with <see cref="IUnitOfWork"/> for transactional
/// operations. The concrete implementation is provided by the Geneirodan.EntityFrameworkCore package.
/// </summary>
/// <remarks>
/// <seealso cref="IUnitOfWork"/> — use together for transactional persistence.<br/>
/// <seealso cref="IEntity{TKey}"/> — <typeparamref name="TEntity"/> must implement this interface.
/// </remarks>
/// <typeparam name="TEntity">The type of the entity that the repository manages.</typeparam>
/// <typeparam name="TKey">The type of the primary key of the entity; must implement <see cref="IEquatable{TKey}"/>.</typeparam>
public interface IRepository<TEntity, in TKey>
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Looks up an entity by identifier and returns <see langword="null"/> if it does not exist instead of throwing.
    /// </summary>
    Task<TEntity?> FindAsync(TKey id, CancellationToken token = default);

    /// <summary>Asynchronously determines whether an entity with the specified identifier exists.</summary>
    Task<bool> ExistsAsync(TKey id, CancellationToken token = default);

    /// <summary>Asynchronously adds the entity to the repository.</summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Asynchronously updates the entity in the repository.</summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Asynchronously deletes the entity from the repository.</summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
