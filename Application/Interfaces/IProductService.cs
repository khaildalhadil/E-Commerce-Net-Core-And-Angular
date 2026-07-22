using Domain.Entities;

namespace Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    Task<bool> ProductExists(int id);
    Task<bool> SaveChangesAsync();
}
