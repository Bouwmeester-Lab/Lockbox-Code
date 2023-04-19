using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Models.Contexts
{
    ///<inheritdoc/>
    public class SqlContext : DataContext
    {
        ///<inherit/>
        public SqlContext(IOptions<DatabaseConfigurationOptions> databaseConfigurationOptions) : base(databaseConfigurationOptions)
        {
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_databaseConfiguration.ConnectionString);
        }
    }
    /// <summary>
    /// Factory for creating migrations at design time.
    /// </summary>
    public class SqlContextContextFactory : IDesignTimeDbContextFactory<SqlContext>
    {
        ///<inheritdoc/>
        public SqlContext CreateDbContext(string[] args)
        {
            var options = new DatabaseConfigurationOptions
            {
                ConnectionString = "Server=localhost,1633;Database=LockboxControlDb;User=sa;TrustServerCertificate=True;Password=Rk+HmfhRN9mt#gm-h*vW8Wbj#",

            };
            return new SqlContext(Options.Create(options));
        }
    }
}
