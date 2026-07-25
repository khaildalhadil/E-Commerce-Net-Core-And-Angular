using Domain.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.Dtos.products;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductsController(IProductService _productService, ILogger<ProductsController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetProducts(string? brand,string? type, string? sort)
    {

        //brand, type, sort
        IReadOnlyList<Product> allProduct = await _productService.GetProductsAsync(brand, type, sort);

        if (allProduct.Count == 0)
        {
            return NotFound(new {Message = "You Don't Have Any Product Yet"});
        }

        // convert to dtos
        IReadOnlyList<ProductDto> productDtos = allProduct.Select(ProductDto.FromEntity).ToList();

        logger.LogInformation("Get All Products ✔");
        return Ok(allProduct);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if(product is null)
        {
            logger.LogWarning($"No Uesr With Id {id}");
            return NotFound();
        }

        return Ok(ProductDto.FromEntity(product));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoroduct(CreateProductDtos createProductDtos)
    {
        // check validation

        var product = CreateProductDtos.ToEntity(createProductDtos);

        _productService.AddProduct(product);

        if (await _productService.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
        }

        return BadRequest();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (!await _productService.ProductExists(id))
        {
            return NotFound();
        }

        if (product.Id != id)
        {
            return BadRequest("Cannt update this product");
        }


        _productService.UpdateProduct(product);

        if (await _productService.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        return BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (!await _productService.ProductExists(id) || product is null)
        {
            return NotFound();
        }


        _productService.DeleteProduct(product);

        if (await _productService.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest();


    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        return Ok(await _productService.GetBrandsAsync());
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        return Ok(await _productService.GetTypesAsync());
    }

    //public async Task<bool> ProductExists(int id)
    //{
    //    return await _productService.ProductExists(id);
    //}

}
