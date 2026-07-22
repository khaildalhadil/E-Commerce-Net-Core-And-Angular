using Domain.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;
    

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        IReadOnlyList<Product> allProduct = await _productService.GetProductsAsync();

        if (allProduct.Count == 0)
        {
            return NotFound(new {Message = "You Don't Have Any Product Yet"});
        }

        return Ok(allProduct);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if(product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoroduct(Product product)
    {
        _productService.AddProduct(product);

        if (await _productService.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
        }

        return BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (await _productService.GetProductByIdAsync(id) is null)
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

    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product is null)
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


    public async Task<bool> ProductExists(int id)
    {
        return await _productService.ProductExists(id);
    }


}
