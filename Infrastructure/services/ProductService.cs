using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.services;

public class ProductService(StoreContext storeContext) : IProductService
{
    private readonly StoreContext _storeContext = storeContext;

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

        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _storeContext.Products.FindAsync(id);
    }
    
    public void AddProduct(Product product)
    {
        _storeContext.Products.Add(product);
    }
    public void UpdateProduct(Product product)
    {
        _storeContext.Entry(product).State = EntityState.Modified;
    }

    public void DeleteProduct(Product product)
    {
        _storeContext.Products.Remove(product);
    }

    public async Task<bool> ProductExists(int id)
    {
        return await _storeContext.Products.AnyAsync(p=> p.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _storeContext.SaveChangesAsync() > 0;
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
