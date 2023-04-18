using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Contracts
{

    /// <summary>
    /// A generic repository service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryableRepositoryService<T> : IRepositoryService<T>, IDisposable
        where T : IEntity
    {
        /// <summary>
        /// Query all items related to the current datakey from the user.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> QueryAll(IServiceScope serviceScope);

        /// <summary>
        /// Gets all items related to the current datakey from the user.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> QueryAll();
        /// <summary>
        /// Takes last count items.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IQueryable<T> Take(int count);
        /// <summary>
        /// Gets a page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IQueryable<T> GetPage(int page, int pageSize, Expression<Func<T, bool>>? filterPredicate = null);

        /// <summary>
        /// Saves any tracked changes.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
