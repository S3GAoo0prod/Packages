using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Geneirodan.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core implementation of <see cref="IUnitOfWork"/> that wraps a <see cref="DbContext"/>.
/// Register with the same lifetime as the DbContext (typically scoped). Call <see cref="SaveChangesAsync"/> after repository
/// operations to persist changes; use <see cref="BeginTransactionAsync"/> when multiple operations must commit or roll back together.
/// </summary>
[PublicAPI]
public class UnitOfWork(DbContext context) : IUnitOfWork
{
    /// <inheritdoc/>
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        new Transaction(await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false));

    /// <inheritdoc/>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Wraps an EF Core <see cref="IDbContextTransaction"/> so that it implements <see cref="ITransaction"/>.
    /// Commit via <see cref="CommitAsync"/>; disposal without commit results in rollback.
    /// </summary>
    public class Transaction(IDbContextTransaction dbContextTransaction) : ITransaction
    {
        /// <inheritdoc/>
        public async Task CommitAsync(CancellationToken cancellationToken = default) =>
            await dbContextTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return dbContextTransaction.DisposeAsync();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContextTransaction.Dispose();
        }
    }
}