using Geneirodan.Abstractions.Domain;
using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Geneirodan.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core implementation of <see cref="IRepository{TEntity, TKey}"/> that uses a <see cref="DbContext"/> and its <see cref="DbSet{TEntity}"/>.
/// Register as scoped (or same lifetime as the DbContext); use together with <see cref="UnitOfWork"/> so that <see cref="IUnitOfWork.SaveChangesAsync"/> persists changes.
/// Override <see cref="FindAsync(IQueryable{TEntity}, TKey, CancellationToken)"/> in derived types to add includes or filtering.
/// </summary>
/// <param name="context">The DbContext that owns the <see cref="DbSet{TEntity}"/> for <typeparamref name="TEntity"/>.</param>
/// <typeparam name="TEntity">The entity type; must be a class and implement <see cref="IEntity{TKey}"/> and be mapped in the context.</typeparam>
/// <typeparam name="TKey">The type of the primary key; must implement <see cref="IEquatable{TKey}"/>.</typeparam>
[PublicAPI]
public class Repository<TEntity, TKey>(DbContext context)
    : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// The DbSet for <typeparamref name="TEntity"/> in the context. Exposed for derived repositories that need to build custom queries.
    /// </summary>
    protected DbSet<TEntity> Set => context.Set<TEntity>();

    /// <inheritdoc/>
    public virtual Task<TEntity?> FindAsync(TKey id, CancellationToken token = default) => FindAsync(Set, id, token);

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(TKey id, CancellationToken token = default) =>
        Set.AnyAsync(e => e.Id.Equals(id), token);

    /// <summary>
    /// Looks up an entity by id in the given queryable. Override to add <c>Include</c> or apply filters before the lookup.
    /// </summary>
    /// <param name="entities">The queryable (e.g. <see cref="Set"/> or an included query).</param>
    /// <param name="id">The entity identifier.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The entity if found; otherwise <see langword="null"/>.</returns>
    protected virtual async Task<TEntity?> FindAsync(IQueryable<TEntity> entities, TKey id, CancellationToken token) =>
        await entities.FirstOrDefaultAsync(e => e.Id.Equals(id), token).ConfigureAwait(false);

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