using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Models;
using LockBoxControl.Core.Frontend.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Frontend.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureHubClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HubOptions>(configuration.GetSection(nameof(HubOptions)));

            services.AddScoped<IStatusHubClient, StatusHubClient>();

            return services;
        }
    }
}
