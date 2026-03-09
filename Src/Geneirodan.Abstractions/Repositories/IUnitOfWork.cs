using JetBrains.Annotations;

namespace Geneirodan.Abstractions.Repositories;

/// <summary>
/// Defines an interface for unit-of-work pattern, managing transactions and saving changes.
/// </summary>
[PublicAPI]
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task{ITransaction}"/> representing the asynchronous operation, with an <see cref="ITransaction"/> that manages the transaction.
    /// </returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made during the current unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}