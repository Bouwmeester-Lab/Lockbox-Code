using LockboxControl.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Services.Base
{
    /// <summary>
    /// Base crud service which takes an IDbContextFactory<TContext> and provides basic operations on the the context. It supports async operations.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericCrudService<TContext, TEntity, Tid> : IGenericRepositoryService<TEntity, Tid>
        where TContext : DbContext
        where TEntity : class, IEntity<Tid>
        where Tid : IEquatable<Tid>
    {
        protected IDbContextFactory<TContext> _dbContextFactory;
        public GenericCrudService(IDbContextFactory<TContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public virtual async Task<TEntity?> GetAsync(Tid id, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public virtual async Task DeleteAsync(Tid id, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            var originalEntity = await context.Set<TEntity>().Where(a => a.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (originalEntity != null)
            {
                context.Set<TEntity>().Remove(originalEntity);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

            var originalEntity = await context.Set<TEntity>().Where(a => a.Id.Equals(entity.Id)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (originalEntity != null)
            {
                context.Entry(originalEntity).CurrentValues.SetValues(entity);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else
                throw new ArgumentOutOfRangeException(nameof(entity), "Entity is not found in the database. Cannot update it.");

        }

        public virtual async Task<List<TEntity>> TakeAsync<Torder>(int count, Expression<Func<TEntity, Torder>> orderExpression, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().OrderByDescending(e => e.Id).Take(count).OrderBy(orderExpression).ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<List<TEntity>> GetPageAsync<Torder>(int page, int pageSize, Expression<Func<TEntity, Torder>> orderExpression, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().OrderByDescending(orderExpression).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
