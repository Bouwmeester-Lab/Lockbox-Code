using LockboxControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Models.Contexts
{
    public class DataContext : BaseDbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DataContext(IOptions<DatabaseConfigurationOptions> options) : base(options)
        {
        }

        protected DataContext()
        {
        }

        public DbSet<Command> Commands => Set<Command>();
        public DbSet<Arduino> Arduinos => Set<Arduino>();
        public DbSet<Request> Requests => Set<Request>();
    }
}
