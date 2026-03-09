using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Geneirodan.EntityFrameworkCore;

/// <inheritdoc />
[PublicAPI]
public class UnitOfWork(DbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        new Transaction(await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);


    /// <inheritdoc cref="IDbContextTransaction"/>
    /// <remarks />
    public class Transaction(IDbContextTransaction dbContextTransaction) : ITransaction
    {
        /// <inheritdoc cref="IDbContextTransaction.CommitAsync(CancellationToken)"/>
        /// <remarks />
        public async Task CommitAsync(CancellationToken cancellationToken = default) =>
            await dbContextTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return dbContextTransaction.DisposeAsync();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContextTransaction.Dispose();
        }
    }
}