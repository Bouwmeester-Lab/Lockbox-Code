using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Contracts
{
    /// <summary>
    /// A generic repository service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepositoryService<T> where T : IEntity
    {
        /// <summary>
        /// Gets an item. Returns null if not present.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T?> GetAsync(long id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Takes last count items.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> TakeAsync(int count, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates an item
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns the created entity.</returns>
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes an item.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    }
}
