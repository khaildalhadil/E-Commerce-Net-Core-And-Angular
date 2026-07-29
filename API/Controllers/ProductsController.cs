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
            logger.LogInformation(
                "No products matched filters Brand={Brand} Type={Type} Sort={Sort}",
                brand, type, sort);
            return NotFound(new {Message = "You Don't Have Any Product Yet"});
        }

        // convert to dtos
        IReadOnlyList<ProductDto> productDtos = allProduct.Select(ProductDto.FromEntity).ToList();

        return Ok(allProduct);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if(product is null)
        {
            logger.LogWarning("Product {ProductId} not found", id);
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
            logger.LogInformation(
                "Created product {ProductId} ({ProductName}) brand {Brand}",
                product.Id, product.Name, product.Brand);
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
        }

        logger.LogWarning("Create product {ProductName} persisted no changes", product.Name);
        return BadRequest();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, CreateProductDtos createProductDtos)
    {
        if (!await _productService.ProductExists(id))
        {
            logger.LogWarning("Update rejected: product {ProductId} not found", id);
            return NotFound();
        }

        var product = CreateProductDtos.ToEntity(createProductDtos);

        if (product.Id != id)
        {
            logger.LogWarning(
                "Update rejected: route id {RouteId} does not match body id {BodyId}",
                id, product.Id);
            return BadRequest("Cannt update this product");
        }


        _productService.UpdateProduct(product);

        if (await _productService.SaveChangesAsync())
        {
            logger.LogInformation("Updated product {ProductId}", id);
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        logger.LogWarning("Update of product {ProductId} persisted no changes", id);
        return BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (!await _productService.ProductExists(id) || product is null)
        {
            logger.LogWarning("Delete rejected: product {ProductId} not found", id);
            return NotFound();
        }


        _productService.DeleteProduct(product);

        if (await _productService.SaveChangesAsync())
        {
            logger.LogInformation("Deleted product {ProductId} ({ProductName})", id, product.Name);
            return NoContent();
        }

        logger.LogWarning("Delete of product {ProductId} persisted no changes", id);
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
