
namespace Domain.Entities;
public class ShoppingCart
{
    public required string Id { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = [];
}

