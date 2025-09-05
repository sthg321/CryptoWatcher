using Ardalis.Specification;

namespace CryptoWatcher.Abstractions;

/// <summary>
/// Represents a generic repositoryFacade interface for managing entities that supports querying,
/// addition, update, deletion, and batch operations. Provides access to a unit of work for transaction management.
/// </summary>
/// <typeparam name="TEntity">The type of the entity that the repositoryFacade manages.</typeparam>
public interface IRepository<TEntity> : IReadRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Inserts a new entity asynchronously into the repositoryFacade.
    /// </summary>
    /// <param name="entity">The entity to be inserted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the inserted entity.</returns>
    TEntity Insert(TEntity entity);

    /// <summary>
    /// Merges a collection of entities into the repositoryFacade by inserting or updating them as needed.
    /// </summary>
    /// <param name="entities">The list of entities to be merged.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task BulkMergeAsync(IList<TEntity> entities, CancellationToken ct);
    
    /// <summary>
    /// Updates the specified entity in the underlying data store.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes the specified entity from the repositoryFacade.
    /// </summary>
    /// <param name="entity">The entity to be deleted. It must be an instance of the type managed by the repositoryFacade.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Gets the instance of <see cref="IUnitOfWork"/> associated with the repositoryFacade.
    /// Provides transaction management capabilities, including the ability to commit, rollback, and save changes
    /// across multiple repositoryFacade operations in a single logical unit of work.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}