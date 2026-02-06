using System.Linq.Expressions;

namespace Cdr.Api.Interfaces;

/// <summary>
/// Interface for a read-only repository for MongoDB.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IReadonlyMongoRepository<T>
{
    /// <summary>
    /// Gets an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
    Task<T> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Gets all entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities that match the predicate.</returns>
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities that match the predicate.</returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets a single entity that matches the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the predicate.</returns>
    Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Checks if any entity exists that matches the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether any entity exists that matches the predicate.</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets the count of all entities asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
    Task<long> CountAsync();

    /// <summary>
    /// Gets the count of entities that match the specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities that match the predicate.</returns>
    Task<long> CountAsync(Expression<Func<T, bool>> predicate);
}