using JetBrains.Annotations;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Represents a database transaction started via <see cref="IUnitOfWork.BeginTransactionAsync"/>.
/// Call <see cref="CommitAsync"/> to commit; if the instance is disposed without committing, the transaction is rolled back.
/// Must be disposed (or disposed asynchronously) to release the underlying connection.
/// </summary>
[PublicAPI]
public interface ITransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Asynchronously commits the transaction and makes all changes made since <see cref="IUnitOfWork.BeginTransactionAsync"/> visible.
    /// After commit, the transaction is completed; disposal no longer performs a rollback.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}