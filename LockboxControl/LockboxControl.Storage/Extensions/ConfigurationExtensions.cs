using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using LockboxControl.Storage.Models;
using LockboxControl.Storage.Models.Contexts;
using LockboxControl.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Storage.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureStorage(this IServiceCollection services, DatabaseConfigurationOptions databaseConfigurationOptions)
        {
            //Add the database:
            switch (databaseConfigurationOptions.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    services.AddDbContext<DataContext, SqlContext>();
                    break;
                case DatabaseType.MySQL:
                    services.AddDbContext<DataContext, MySqlContext>();
                    break;
                case DatabaseType.SQLite:
                    services.AddDbContext<DataContext, SqliteContext>();
                    break;
            }
            services.AddDbContextFactory<DataContext, DataContextFactory>();

            services.AddScoped<IQueryableRepositoryService<Arduino>, CrudService<DataContext, Arduino>>();
            services.AddScoped<IQueryableRepositoryService<Command>, CrudService<DataContext, Command>>();

            return services;
        }
    }
}
