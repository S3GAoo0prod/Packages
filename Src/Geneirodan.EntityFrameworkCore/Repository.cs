using Geneirodan.Abstractions.Domain;
using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Geneirodan.EntityFrameworkCore;

/// <summary>
/// Provides an implementation of the repository pattern for managing entities in a DbContext.
/// </summary>
/// <param name="context">
/// The <see cref="DbContext"/> instance used to interact with the database.
/// </param>
/// <typeparam name="TEntity">
/// The type of the entity that the repository manages.
/// It must implement the <see cref="IEntity{TKey}"/> interface.
/// </typeparam>
/// <typeparam name="TKey">
/// The type of the primary key of the entity. It must implement <see cref="IEquatable{TKey}"/>.
/// </typeparam>
[PublicAPI]
public class Repository<TEntity, TKey>(DbContext context)
    : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// The DbSet representing the collection of <typeparamref name="TEntity"/> entities in the context.
    /// </summary>
    protected DbSet<TEntity> Set => context.Set<TEntity>();

    /// <inheritdoc/>
    public virtual Task<TEntity?> FindAsync(TKey id, CancellationToken token = default) => FindAsync(Set, id, token);

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(TKey id, CancellationToken token = default) => 
        Set.AnyAsync(e => e.Id.Equals(id), token);

    /// <inheritdoc cref="IRepository{TEntity,TKey}.FindAsync"/>
    protected virtual async Task<TEntity?> FindAsync(IQueryable<TEntity> entities, TKey id, CancellationToken token) => 
        await entities.FirstOrDefaultAsync(e => e.Id.Equals(id),token).ConfigureAwait(false);

    /// <inheritdoc/>
    public virtual Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default) => 
        Task.FromResult(Set.Add(entity).Entity);

    /// <inheritdoc/>
    public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) => 
        Task.FromResult(Set.Update(entity).Entity);

    /// <inheritdoc/>
    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) => 
        Task.FromResult(Set.Remove(entity));
}