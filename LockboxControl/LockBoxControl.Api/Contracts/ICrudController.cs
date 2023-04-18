using LockboxControl.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Contracts
{
    public interface ICrudController<T>
        where T : IEntity
    {
        /// <summary>
        /// Gets all the items.
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Get a single item by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Save an item.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the created entity with id.</returns>
        public Task<ActionResult<T>> SaveAsync(T entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates an item by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<IActionResult> UpdateAsync(long id, T entity, CancellationToken cancellationToken = default);
    }
}
