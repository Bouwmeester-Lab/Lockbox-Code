using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Services;
using LockBoxControl.Core.Models;

namespace LockBoxControl.Blazor.Client.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApiClient<TEntity, TId>(this IServiceCollection services, IConfiguration configuration)
            where TEntity : IEntity<TId>
            where TId : IEquatable<TId>
        {
            services.AddHttpClient<ICrudClient<TEntity, TId>, CrudClient<TEntity, TId>>(client =>
            {
                client.BaseAddress = new Uri(configuration["BaseApiAddress"]
                    ?? throw new ArgumentNullException("BaseApiAddress"));
            });

            return services;
        }

        public static IServiceCollection AddApiClient<TEntity>(this IServiceCollection services, IConfiguration configuration)
            where TEntity : IEntity
        {
            services.AddApiClient<TEntity, long>(configuration);
            return services;
        }
    }
}
