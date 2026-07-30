using Domain.Entities;

namespace Application.Interfaces;

public interface ICartService
{
    Task<ShoppingCart?> GetCarAsync(string key);
    Task<ShoppingCart?> SetCartAsync(ShoppingCart cart);
    Task<bool> DeleteCartAsync(string key);
}
