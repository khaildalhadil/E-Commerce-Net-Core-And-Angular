using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.products;
public class CreateProductDtos
{
    [Required]
    [StringLength(40, MinimumLength =2)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string Description { get; set; } = string.Empty;

    [Range(0.001, double.MaxValue, ErrorMessage = "Price must be greater then 0")]
    public decimal Price { get; set; }

    [Required]
    public string PictureUrl { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Range(0.001, int.MaxValue, ErrorMessage = "Quantitiy In Stock must be greater then 0")]
    public int QuantitiyInStock { get; set; }


    public static Product ToEntity(CreateProductDtos productDtos)
    {
        return new Product()
        {
            Name = productDtos.Name,
            Description = productDtos.Description,
            Price = productDtos.Price,
            PictureUrl = productDtos.PictureUrl,
            Type = productDtos.Type,
            Brand = productDtos.Brand,
            QuantitiyInStock = productDtos.QuantitiyInStock
        };
    }
}
