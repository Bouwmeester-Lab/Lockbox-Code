using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;
using LockBoxControl.Storage.Models;
using LockBoxControl.Storage.Models.Contexts;
using LockBoxControl.Storage.Services;
using LockBoxControl.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Storage.Extensions
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
            services.AddScoped<IGenericQueryableRepositoryService<Request, Guid>, QueryableCrudService<DataContext, Request, Guid>>();
            services.AddScoped<IGenericQueryableRepositoryService<ArduinoStatus, Guid>, QueryableCrudService<DataContext, ArduinoStatus, Guid>>();

            //services.AddScoped<IGenericQueryableRepositoryService<Arduino, long>, QueryableCrudService<DataContext, Arduino, long>>();


            return services;
        }
    }
}
