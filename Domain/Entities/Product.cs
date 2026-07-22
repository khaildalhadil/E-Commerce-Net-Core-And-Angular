namespace Domain.Entities;

public class Product: BaseEntity
{
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public required string PictureUrl { get; set; } = string.Empty;
    public required string Type { get; set; } = string.Empty;
    public required string Brand { get; set; } = string.Empty;
    public int QuantitiyInStock { get; set; }
}
