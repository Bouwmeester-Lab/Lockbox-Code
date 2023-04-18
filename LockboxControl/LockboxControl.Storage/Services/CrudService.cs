using LockboxControl.Core.Contracts;
using LockboxControl.Storage.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Services
{
    public class CrudService<TContext, TEntity> : BaseCrudService<TContext, TEntity>, IQueryableRepositoryService<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {
        protected readonly TContext _context;
        public CrudService(TContext context, IDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
        {
            _context = context;
        }

        public IQueryable<TEntity> QueryAll(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
            return context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetPage(int page, int pageSize, Expression<Func<TEntity, bool>>? filterPredicate = null)
        {
            if (filterPredicate is not null)
            {
                return _context.Set<TEntity>().OfType<TEntity>().Where(filterPredicate).OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize);
            }
            return _context.Set<TEntity>().OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<TEntity> QueryAll()
        {
            return _context.Set<TEntity>();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<TEntity> Take(int count)
        {
            return _context.Set<TEntity>().OrderBy(x => x.Id).Take(count);
        }

        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}
