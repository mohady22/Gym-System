using GymSystem.DAL.Contexts;
using GymSystem.DAL.DataSeeds;
using Microsoft.EntityFrameworkCore;

namespace A_MVC01
{
    public static class ProgramExtentions
    {
        public static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var configurations = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var pending = await dbContext.Database.GetPendingMigrationsAsync();

            if (pending.Any())
            {
                logger.LogInformation($"Apply {pending.Count()} Pending Migrations");
                await dbContext.Database.MigrateAsync();
            }

            var seedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");

            await GymDataSeed.SeedAsync(dbContext, seedPath,logger);
        }
    }
}
