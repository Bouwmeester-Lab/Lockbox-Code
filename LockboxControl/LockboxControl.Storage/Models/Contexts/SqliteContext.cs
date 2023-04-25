using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Storage.Models.Contexts
{
    public class SqliteContext : DataContext
    {
        ///<inherit/>
        public SqliteContext(IOptions<DatabaseConfigurationOptions> databaseConfigurationOptions) : base(databaseConfigurationOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(_databaseConfiguration.ConnectionString);
        }
    }
}
