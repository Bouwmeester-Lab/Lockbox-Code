using Microsoft.Extensions.DependencyInjection;
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
    public interface IGenericQueryableRepositoryService<TEntity, TId> : IGenericRepositoryService<TEntity, TId>, IDisposable
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Query all items related to the current datakey from the user.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> QueryAll(IServiceScope serviceScope);

        /// <summary>
        /// Gets all items related to the current datakey from the user.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> QueryAll();
        /// <summary>
        /// Takes last count items.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IQueryable<TEntity> Take<TOrder>(int count, Expression<Func<TEntity, TOrder>> orderExpression);
        /// <summary>
        /// Gets a page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetPage<TOrder>(int page, int pageSize, Expression<Func<TEntity, TOrder>> orderExpression, Expression<Func<TEntity, bool>>? filterPredicate = null);

        /// <summary>
        /// Saves any tracked changes.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
