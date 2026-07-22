using Domain.Entities;
using System.Text.Json;

namespace Infrastructure.Database;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var productsDate = await File.ReadAllTextAsync("../Infrastructure/SeedDate/products.json");

            // from json to list
            var products = JsonSerializer.Deserialize<List<Product>>(productsDate);

            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
