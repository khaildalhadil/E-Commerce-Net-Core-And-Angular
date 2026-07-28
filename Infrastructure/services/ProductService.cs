using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.services;

public class ProductService(StoreContext storeContext, ILogger<ProductService> logger) : IProductService
{
    private readonly StoreContext _storeContext = storeContext;

    // PostgreSQL unique_violation - lets a duplicate be reported as a warning instead of an error.
    private const string UniqueViolation = "23505";

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brands, string? type, string? sort)
    {
        var query = _storeContext.Products.AsQueryable();

        if (brands?.Length > 0)
        {
            string[] brandsArray = brands.Split(",");
            //query = query.Where(p => p.Brand == brand);
            for (var i = 0; i < brandsArray.Length; i++)
            {
                query = query.Where(p => p.Brand == brandsArray[i]);
            }
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(p => p.Type == type);
        }

        query = sort switch
        {
            "priceAsc" => query.OrderBy(b => b.Price),
            "priceDesc" => query.OrderByDescending(b => b.Price),
            _ => query.OrderBy(n => n.Name)
        };

        var products = await query.ToListAsync();

        // Debug level: useful when diagnosing a filter, but far too frequent for production Information.
        logger.LogDebug(
            "Product query Brand={Brand} Type={Type} Sort={Sort} returned {ProductCount} products",
            brands, type, sort, products.Count);

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var product = await _storeContext.Products.FindAsync(id);

        if (product is null)
        {
            logger.LogDebug("Product {ProductId} not found in database", id);
        }

        return product;
    }

    public void AddProduct(Product product)
    {
        logger.LogInformation("Adding product {ProductName} (brand {Brand}) to the change tracker", product.Name, product.Brand);
        _storeContext.Products.Add(product);
    }
    public void UpdateProduct(Product product)
    {
        logger.LogInformation("Marking product {ProductId} as modified", product.Id);
        _storeContext.Entry(product).State = EntityState.Modified;
    }

    public void DeleteProduct(Product product)
    {
        logger.LogInformation("Marking product {ProductId} for deletion", product.Id);
        _storeContext.Products.Remove(product);
    }

    public async Task<bool> ProductExists(int id)
    {
        return await _storeContext.Products.AnyAsync(p=> p.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        try
        {
            var affected = await _storeContext.SaveChangesAsync();

            if (affected == 0)
            {
                // Silent no-op saves are the usual cause of an unexplained BadRequest upstream.
                logger.LogWarning("SaveChanges completed without affecting any rows");
            }

            return affected > 0;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Concurrency conflict while saving product changes");
            throw;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: UniqueViolation } pg)
        {
            logger.LogWarning(ex, "Duplicate value rejected by constraint {ConstraintName}", pg.ConstraintName);
            throw;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed while saving product changes");
            throw;
        }
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _storeContext.Products.Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await _storeContext.Products.Select(p => p.Type)
            .Distinct()
            .ToListAsync();
    }
}
