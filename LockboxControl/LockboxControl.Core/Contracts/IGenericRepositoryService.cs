using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Contracts
{
    /// <summary>
    /// A generic repository service.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericRepositoryService<TEntity, Tid> where TEntity : IEntity<Tid>
    {
        /// <summary>
        /// Gets an item. Returns null if not present.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TEntity?> GetAsync(Tid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Takes last count items.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TEntity>> TakeAsync<Torder>(int count, Expression<Func<TEntity, Torder>> orderExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetPageAsync<Torder>(int page, int pageSize, Expression<Func<TEntity, Torder>> orderExpression, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates an item
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns the created entity.</returns>
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes an item.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(Tid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
