using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Models
{
    public abstract class ContextFactory<TContext, TOptions> : IDbContextFactory<TContext>
        where TContext : DbContext
        where TOptions : DatabaseConfigurationOptions
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IOptions<TOptions> _options;

        public ContextFactory(IServiceProvider serviceProvider,
            IOptions<TOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }
        public virtual TContext CreateDbContext()
        {
            switch (_options.Value.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    return CreateSqlDbContext(_options);
                case DatabaseType.MySQL:
                    return CreateMySqlDbContext(_options);
                case DatabaseType.SQLite:
                    return CreateSqliteContext(_options);
                default:
                    throw new InvalidOperationException("Unknown type of database.");
            }
        }

        protected abstract TContext CreateMySqlDbContext(IOptions<TOptions> options);
        protected abstract TContext CreateSqlDbContext(IOptions<TOptions> options);
        protected abstract TContext CreateSqliteContext(IOptions<TOptions> options);
    }
}
