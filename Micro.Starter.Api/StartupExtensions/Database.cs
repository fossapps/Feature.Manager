using System;
using System.Threading.Tasks;
using Micro.Starter.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Micro.Starter.Api.StartupExtensions
{
    public static class Database
    {
        public static async Task MigrateOrFail(this DatabaseFacade db, ILogger<ApplicationContext> logger)
        {
            for (var i = 0; i <= 3; i++)
            {
                var waitTime = new[] {1, 3, 8, 10}[i];
                logger.LogInformation($"Db connection attempt in {waitTime} seconds");
                await Task.Delay(TimeSpan.FromSeconds(waitTime));

                if (await TryMigrate(db))
                {
                    return;
                }
                logger.LogWarning("Connection failed...");
            }
            throw new RetryLimitExceededException("Couldn't connect to database");
        }
        private static async Task<bool> TryMigrate(DatabaseFacade db)
        {
            try
            {
                var canConnect = await db.CanConnectAsync();
                if (!canConnect)
                {
                    return false;
                }
                await db.MigrateAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}