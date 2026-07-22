using Domain.Entities;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.services;

namespace API.Controllers;

[ApiController]
///[action]
[Route("api/[controller]")]
public class ProductsController(IProductService _productService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetProducts(string? brand,string? type, string? sort)
    {
        //brand, type, sort
        IReadOnlyList<Product> allProduct = await _productService.ListAllAsync();

        if (allProduct.Count == 0)
        {
            return NotFound(new {Message = "You Don't Have Any Product Yet"});
        }

        return Ok(allProduct);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if(product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoroduct(Product product)
    {
        _productService.Add(product);

        if (await _productService.SaveAllAsync())
        {
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
        }

        return BadRequest();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (!_productService.Exsits(id))
        {
            return NotFound();
        }

        if (product.Id != id)
        {
            return BadRequest("Cannt update this product");
        }


        _productService.Update(product);

        if (await _productService.SaveAllAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        return BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {


        var product = await _productService.GetByIdAsync(id);

        if (!_productService.Exsits(id) || product is null)
        {
            return NotFound();
        }


        _productService.Delete(product);

        if (await _productService.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest();


    }

    //[HttpGet("brands")]
    //public async Task<IActionResult> GetBrands()
    //{
    //    return Ok(await _productService.GetBrandsAsync());
    //}

    //[HttpGet("types")]
    //public async Task<IActionResult> GetTypes()
    //{
    //    return Ok(await _productService.GetTypesAsync());
    //}

    //[HttpGet("")]
    //public async Task<IActionResult> GetBrandQuery([FromQuery] string brand)
    //{
    //    return Ok(new { message = brand });
    //}

    //public async Task<bool> ProductExists(int id)
    //{
    //    return await _productService.ProductExists(id);
    //}


}
