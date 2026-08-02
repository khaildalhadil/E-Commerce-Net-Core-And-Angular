using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TextController(
    ILogger<TextController> _logger,
    IConfiguration _configuration,
    IHttpClientFactory _httpClientFactory) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] ProductTextRequest request)
    {
        var result = await GenerateProductTextAsync(request.ProductName);
        return Ok(result);
    }

    private async Task<ProductTextResponse> GenerateProductTextAsync(string productName)
    {
        var apiKey = _configuration["OpenAIApiKey"]
            ?? throw new InvalidOperationException("OpenAIApiKey is not configured.");

        var http = _httpClientFactory.CreateClient("OpenAI");

        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "gpt-4o-mini",
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = "You generate random, realistic e-commerce product listing data. " +
                        "Reply with strict JSON only, no markdown, using exactly these keys: " +
                        "description (string, 20-40 words), price (number, 5-500), type (string, a product category), " +
                        "brand (string), quantityInStock (integer, 1-200)."
                },
                new
                {
                    role = "user",
                    content = $"Product name: \"{productName}\". Generate the JSON now."
                }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await http.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("OpenAI text generation failed: {Response}", responseJson);
            throw new Exception(responseJson);
        }

        using var doc = JsonDocument.Parse(responseJson);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;

        var result = JsonSerializer.Deserialize<ProductTextResponse>(
            messageContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result is null)
            throw new Exception("OpenAI returned a response that could not be parsed.");

        return result;
    }
}

public class ProductTextRequest
{
    public string ProductName { get; set; } = "";
}

public class ProductTextResponse
{
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string Type { get; set; } = "";
    public string Brand { get; set; } = "";
    public int QuantityInStock { get; set; }
}
