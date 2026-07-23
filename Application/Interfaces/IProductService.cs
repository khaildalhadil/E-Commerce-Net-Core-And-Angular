using Application.Dtos.products;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort);
    Task<Product?> GetProductByIdAsync(int id);
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);

    Task<bool> ProductExists(int id);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();
    Task<bool> SaveChangesAsync();
}
