using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GymSystem.DAL.DataSeeds
{
    public static class GymDataSeed
    {
        public static async Task SeedAsync(GymDbContext dbContext, string seedFilesPath, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                if (!await dbContext.Plans.AnyAsync(ct))
                {
                    var plans = LoadDataFromJsonFile<Plan>("plans.json", seedFilesPath);

                    if (plans.Count > 0)
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Seeded {plans.Count} plans");
                    }
                }
                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync(ct);
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gym Data Seeding Failed");
                throw;
            }
        }
        private static List<T> LoadDataFromJsonFile<T>(string fileName, string folderPath)
        {
            var filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Seed Data File Not Found: {filePath}");
            }
            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
