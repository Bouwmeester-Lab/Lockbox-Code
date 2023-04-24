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
    public interface IRepositoryService<T> : IGenericRepositoryService<T, long>
        where T : IEntity
    {
        /// <summary>
        /// Takes last count items ordered by the Id.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> TakeAsync(int count, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a page ordered by the id.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
