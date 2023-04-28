using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Frontend.Contracts
{
    public interface ICrudClient<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Gets the number of items available in the server.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> GetCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all the items.
        /// </summary>
        /// <returns></returns>
        public Task<List<TEntity>?> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a page from the items.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<PagedResult<TEntity>?> GetPageAsync(int pageNumber, int pageSize = 10, CancellationToken cancellationToken = default);

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
        public Task<TEntity?> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates an item by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }

    public interface ICrudClient<T> : ICrudClient<T, long>
        where T : IEntity
    {
    }
}
