
using Application.Interfaces;
using Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.services;

public class CartService(IConnectionMultiplexer redis) : ICartService
{
    public readonly IDatabase database = redis.GetDatabase();


    public async Task<ShoppingCart?> GetCarAsync(string key)
    {
        var data = await database.StringGetAsync(key);

        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>((byte[])data!);
    }

    public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
    {
        var created = await database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        if (!created) return null;

        return await GetCarAsync(cart.Id);
    }
    public async Task<bool> DeleteCartAsync(string key)
    {
        return await database.KeyDeleteAsync(key);
    }
}
