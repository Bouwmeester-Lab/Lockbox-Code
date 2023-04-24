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
    public interface IQueryableRepositoryService<T> : IGenericQueryableRepositoryService<T, long>
        where T : IEntity
    {
        /// <summary>
        /// Takes last count items organizing by Id.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IQueryable<T> Take(int count);
        /// <summary>
        /// Gets a page organizing by Id.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IQueryable<T> GetPage(int page, int pageSize, Expression<Func<T, bool>>? filterPredicate = null);
    }
}
