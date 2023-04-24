using LockboxControl.Core.Contracts;
using LockboxControl.Storage.Extensions;
using LockboxControl.Storage.Services.Base;
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
    public class CrudService<TContext, TEntity> : QueryableCrudService<TContext, TEntity, long>, IQueryableRepositoryService<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {
        public CrudService(IDbContextFactory<TContext> dbContextFactory, TContext context) : base(dbContextFactory, context)
        {
        }

        public IQueryable<TEntity> GetPage(int page, int pageSize, Expression<Func<TEntity, bool>>? filterPredicate = null)
        {
            if (filterPredicate is not null)
            {
                return _context.Set<TEntity>().OfType<TEntity>().Where(filterPredicate).OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize);
            }
            return _context.Set<TEntity>().OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public override Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            if (id == 0)
            {
                return Task.CompletedTask; // We skip an entity with id == 0
            }
            return base.DeleteAsync(id, cancellationToken);
        }

        public override Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if(entity.Id == 0)
            {
                return Task.CompletedTask; // We skip an entity with id == 0
            }
            return base.UpdateAsync(entity, cancellationToken);
        }

        public IQueryable<TEntity> Take(int count)
        {
            return base.Take(count, x => x.Id);
        }
    }
}
