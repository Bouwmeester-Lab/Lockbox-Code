using LockboxControl.Core.Contracts;
using LockboxControl.Storage.Models.Contexts;
using LockboxControl.Storage.Services.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Services
{
    public class QueryableCrudService<TContext, TEntity, TId> : GenericCrudService<TContext, TEntity, TId>, IGenericQueryableRepositoryService<TEntity, TId>
        where TContext : DbContext
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected readonly TContext _context;
        public QueryableCrudService(IDbContextFactory<TContext> dbContextFactory, TContext context) : base(dbContextFactory)
        {
            _context = context;
        }
        public IQueryable<TEntity> GetPage<TOrder>(int page, int pageSize, Expression<Func<TEntity, TOrder>> orderExpression, Expression<Func<TEntity, bool>>? filterPredicate = null)
        {
            if(filterPredicate is not null)
            {
                return _context.Set<TEntity>().OfType<TEntity>().Where(filterPredicate).OrderByDescending(orderExpression).Skip((page - 1) * pageSize).Take(pageSize);
            }
            return _context.Set<TEntity>().OrderByDescending(orderExpression).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<TEntity> QueryAll(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
            return context.Set<TEntity>();
        }

        public IQueryable<TEntity> QueryAll()
        {
            return _context.Set<TEntity>();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<TEntity> Take<TOrder>(int count, Expression<Func<TEntity, TOrder>> orderExpression)
        {
            return _context.Set<TEntity>().OrderBy(orderExpression).Take(count);
        }

        public virtual void Dispose()
        {
            _context.Dispose();
        }

    }
}
