using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GymManagement_Shared_Classes.Migrations.NonSeeded
{
    public static class MigrationConfig
    {
        public static async Task ApplyMigrationsAsync<TContext>(this WebApplication app) where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                                           .CreateLogger("EFMigrations");
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<TContext>();
                await db.Database.MigrateAsync();
                log.LogInformation("EF migrations applied for {Context}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error applying EF migrations for {Context}", typeof(TContext).Name);
                throw;
            }
        }
    }
}
