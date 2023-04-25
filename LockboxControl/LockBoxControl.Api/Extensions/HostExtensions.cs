using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LockBoxControl.Api.Extensions
{
    /// <summary>
    /// Extensions for the WebBuilder
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Migrates the given database using EF core.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApplication"></param>
        /// <returns></returns>
        public static async Task<IHost> MigrateDatabaseAsync<T>(this IHost webApplication) where T : DbContext
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<T>();
                    db?.Database.Migrate();
                    // try to add the default commands
                    var commandService = services.GetRequiredService<IQueryableRepositoryService<Command>>();
                    var existingCommand = await commandService.QueryAll().Where(x => x.CommandLetter == Command.MacAddressCommand.CommandLetter).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (existingCommand is null)
                    {
                        // create the command
                        await commandService.CreateAsync(Command.MacAddressCommand).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return webApplication;
        }
    }
}
