using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionConstraint> CreatePoroduct(Product product)
    {
        _productService.AddProduct(product);
        var isSavted = await _productService.SaveChangesAsync();
        
        if (isSavted)
        {
            return Created();
        }

        return BadRequest();
    }
 

}
