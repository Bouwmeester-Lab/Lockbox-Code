using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Models.Contexts
{
    public class BaseDbContext : DbContext
    {
        /// <summary>
        /// Configuration used.
        /// </summary>
        protected readonly DatabaseConfigurationOptions _databaseConfiguration = new DatabaseConfigurationOptions();
        /// <summary>
        /// Constructor to use with DI.
        /// </summary>
        /// <param name="options"></param>
        public BaseDbContext(IOptions<DatabaseConfigurationOptions> options) : base()
        {
            _databaseConfiguration = options.Value;
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected BaseDbContext() : base()
        {
        }
    }
}
