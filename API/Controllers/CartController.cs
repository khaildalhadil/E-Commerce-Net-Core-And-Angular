
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart = await cartService.GetCarAsync(id);

        return Ok(cart ?? new ShoppingCart { Id = id });
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await cartService.SetCartAsync(cart);

        if (updatedCart == null) return BadRequest("Problem with cart");

        return updatedCart;
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCart(string id)
    {
        var result = await cartService.DeleteCartAsync(id);
        if (!result) return BadRequest("problem with delete cart");
        return Ok(result);
    }
}
