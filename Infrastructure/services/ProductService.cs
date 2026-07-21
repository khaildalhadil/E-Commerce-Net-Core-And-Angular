using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.services;

public class ProductService(StoreContext storeContext) : IProductService
{
    private readonly StoreContext _storeContext = storeContext;

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        return await _storeContext.Products.ToListAsync();
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

}
