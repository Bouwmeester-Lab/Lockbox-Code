using LockboxControl.Storage.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Models
{
    public class DataContextFactory : ContextFactory<DataContext, DatabaseConfigurationOptions>, IDbContextFactory<DataContext>
    {
        public DataContextFactory(IServiceProvider serviceProvider, IOptions<DatabaseConfigurationOptions> options) : base(serviceProvider, options)
        {
        }

        protected override DataContext CreateMySqlDbContext(IOptions<DatabaseConfigurationOptions> options)
        {
            return new MySqlContext(options);
        }

        protected override DataContext CreateSqlDbContext(IOptions<DatabaseConfigurationOptions> options)
        {
            return new SqlContext(options);
        }

        protected override DataContext CreateSqliteContext(IOptions<DatabaseConfigurationOptions> options)
        {
            return new SqliteContext(options);
        }
    }
}
