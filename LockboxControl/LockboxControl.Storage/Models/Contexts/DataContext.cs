using LockBoxControl.Core.Models;
using LockBoxControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Storage.Models.Contexts
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
        public DbSet<ArduinoStatus> ArduinoStatuses => Set<ArduinoStatus>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Request>().Navigation(x => x.Arduino).AutoInclude();

            modelBuilder.Entity<Request>().Navigation(x => x.Command).AutoInclude();
        }
    }
}
