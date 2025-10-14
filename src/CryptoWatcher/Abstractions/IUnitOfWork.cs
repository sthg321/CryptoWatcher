namespace CryptoWatcher.Abstractions;

/// <summary>
/// Defines a contract for a Unit of Work pattern that facilitates
/// managing transactions, committing, and rolling back changes in a coherent manner across multiple repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation and provides a disposable object for managing the transaction's lifetime.</returns>
    Task BeginTransactionAsync(CancellationToken ct);

    /// <summary>
    /// Rolls back a pending transaction asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous rollback operation.</returns>
    Task RollbackTransactionAsync(CancellationToken ct);

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation and provides a disposable object for managing the transaction's lifetime.</returns>
    Task CommitTransactionAsync(CancellationToken ct);

    /// <summary>
    /// Saves all changes made in the context to the database asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken ct);
}