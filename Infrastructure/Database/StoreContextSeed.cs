using Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Database;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        // The signature is fixed, so the logger comes from the context's own service provider
        // rather than from constructor injection.
        var logger = context.GetService<ILoggerFactory>().CreateLogger(typeof(StoreContextSeed));

        if (!context.Products.Any())
        {
            const string seedPath = "../Infrastructure/SeedDate/products.json";

            // The path is relative to the working directory, so record which file was actually used.
            logger.LogInformation("Products table is empty, seeding from {SeedFile}", seedPath);

            var productsDate = await File.ReadAllTextAsync(seedPath);

            // from json to list
            var products = JsonSerializer.Deserialize<List<Product>>(productsDate);

            if (products == null)
            {
                logger.LogWarning("Seed file {SeedFile} deserialized to null, skipping seed", seedPath);
                return;
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            logger.LogInformation("Seeded {ProductCount} products", products.Count);
        }
        else
        {
            logger.LogInformation("Products table already populated, skipping seed");
        }
    }
}
