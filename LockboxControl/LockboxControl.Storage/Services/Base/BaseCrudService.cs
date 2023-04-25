using LockBoxControl.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Storage.Services.Base
{
    /// <summary>
    /// Base crud service which takes an IDbContextFactory<TContext> and provides basic operations on the the context. It supports async operations.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseCrudService<TContext, TEntity> : GenericCrudService<TContext, TEntity, long>, IRepositoryService<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {
        public BaseCrudService(IDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        //public BaseCrudService(IDbContextFactory<TContext> dbContextFactory)
        //{
        //    _dbContextFactory = dbContextFactory;
        //}
        //public async Task<TEntity?> GetAsync(long id, CancellationToken cancellationToken = default)
        //{
        //    using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        //    return await context.Set<TEntity>().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        //}

        //public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        //{
        //    using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        //    return await context.Set<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false);
        //}

        //public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        //{
        //    using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        //    context.Set<TEntity>().Add(entity);
        //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //    return entity;
        //}

        //public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        //{
        //    using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        //    var originalEntity = await context.Set<TEntity>().Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        //    if (originalEntity != null)
        //    {
        //        context.Set<TEntity>().Remove(originalEntity);
        //        await context.SaveChangesAsync().ConfigureAwait(false);
        //    }
        //}

        public override async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
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

        public Task<List<TEntity>> TakeAsync(int count, CancellationToken cancellationToken = default)
        {
            return base.TakeAsync(count, x => x.Id, cancellationToken);
        }

        public Task<List<TEntity>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return base.GetPageAsync(page, pageSize, x => x.Id, cancellationToken);
        }
    }
}
