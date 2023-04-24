using LockboxControl.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Contracts
{
    public interface ICrudController<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Gets all the items.
        /// </summary>
        /// <returns></returns>
        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Get a single item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Save an item.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the created entity with id.</returns>
        public Task<ActionResult<TEntity>> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IActionResult> DeleteAsync(TId id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates an item by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<IActionResult> UpdateAsync(TId id, TEntity entity, CancellationToken cancellationToken = default);
    }

    public interface ICrudController<T> : ICrudController<T, long>
        where T : IEntity
    {
    }
}
