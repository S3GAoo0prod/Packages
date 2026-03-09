using JetBrains.Annotations;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Defines an interface for managing transactions with asynchronous commit and rollback operations.
/// </summary>
[PublicAPI]
public interface ITransaction : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Asynchronously commits the transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}