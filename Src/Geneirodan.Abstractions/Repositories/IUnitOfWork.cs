using JetBrains.Annotations;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Defines the unit-of-work pattern: coordinates persistence and explicit database transactions.
/// Use this interface to flush changes from repositories (e.g. <see cref="IRepository{TEntity, TKey}"/>) and to
/// run multiple operations in a single transaction. In web applications, typically one unit of work is scoped per request.
/// </summary>
[PublicAPI]
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// Use the returned <see cref="ITransaction"/> to commit or to let disposal roll back. Call <see cref="SaveChangesAsync"/>
    /// before <see cref="ITransaction.CommitAsync"/> to persist changes within the transaction.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.<br/>
    /// The task result is an <see cref="ITransaction"/> that must be disposed; call <see cref="ITransaction.CommitAsync"/> to commit.
    /// </returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists all changes made in the current unit of work (e.g. repository adds/updates/deletes) asynchronously.
    /// Must be called after repository operations for changes to be written to the database.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}