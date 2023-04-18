using LockboxControl.Core.Contracts;
using LockboxControl.Storage.Extensions;
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
    public class CrudServiceWithDiscriminator<TContext, TEntity> : IQueryableRepositoryService<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {
        protected readonly TContext _context;
        protected readonly IDbContextFactory<TContext> _dbContextFactory;
        public CrudServiceWithDiscriminator(TContext context, IDbContextFactory<TContext> dbContextFactory)
        {
            _context = context;
            _dbContextFactory = dbContextFactory;
        }

        public IQueryable<TEntity> QueryAll(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();
            return context.Set<TEntity>().OfOnlyType();
        }

        public IQueryable<TEntity> GetPage(int page, int pageSize, Expression<Func<TEntity, bool>>? filterPredicate = null)
        {
            if (filterPredicate is not null)
            {
                return _context.Set<TEntity>().OfOnlyType().Where(filterPredicate).OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize);
            }
            return _context.Set<TEntity>().OfOnlyType().OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<TEntity> QueryAll()
        {
            return _context.Set<TEntity>().OfOnlyType();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<TEntity> Take(int count)
        {
            return _context.Set<TEntity>().OfOnlyType().OrderBy(x => x.Id).Take(count);
        }

        public async Task<TEntity?> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().OfOnlyType().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().OfOnlyType().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            var originalEntity = await context.Set<TEntity>().Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (originalEntity != null)
            {
                context.Set<TEntity>().Remove(originalEntity);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (entity.Id != 0)
            {
                var originalEntity = await context.Set<TEntity>().Where(a => a.Id == entity.Id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (originalEntity != null)
                {
                    context.Entry(originalEntity).CurrentValues.SetValues(entity);
                    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(entity), "Entity is not found in the database. Cannot update it.");
            }
        }

        public async Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().OfOnlyType().OrderByDescending(e => e.Id).Take(count).OrderBy(e => e.Id).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<TEntity>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            return await context.Set<TEntity>().OfOnlyType().OrderByDescending(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
