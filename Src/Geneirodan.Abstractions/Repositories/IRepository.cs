using Geneirodan.Abstractions.Domain;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Defines the contract for a repository that manages entities of type <typeparamref name="TEntity"/>
/// with a primary key of type <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity that the repository manages. It must implement the <see cref="IEntity{TKey}"/> interface.
/// </typeparam>
/// <typeparam name="TKey">
/// The type of the primary key of the entity. It must implement <see cref="IEquatable{TKey}"/>.
/// </typeparam>
public interface IRepository<TEntity, in TKey>
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">
    /// The identifier of the entity to retrieve.
    /// </param>
    /// <param name="token">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.<br/>
    /// The task result is the entity with the specified identifier, or <see langword="null"/> if no entity is found.
    /// </returns>
    Task<TEntity?> FindAsync(TKey id, CancellationToken token = default);

    /// <summary>
    /// Asynchronously determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">
    /// The identifier of the entity to check for existence.
    /// </param>
    /// <param name="token">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.<br/>
    /// The task result is <see langword="true"/> if an entity with the specified identifier exists; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> ExistsAsync(TKey id, CancellationToken token = default);

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to add.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.<br/>
    /// The task result is the added entity.
    /// </returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to update.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.<br/>
    /// The task result is the updated entity.
    /// </returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">
    /// The entity to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}