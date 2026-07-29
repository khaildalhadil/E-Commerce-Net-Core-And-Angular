using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController(ILogger<ImageController> _logger, IConfiguration _configuration) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] ImageRequest request)
    {
        string imageUrl = await GenerateImageAsync(request.Prompt);
        var response = new ImageRequest() { Prompt = imageUrl };
        return Ok(response);
    }

    private async Task<string> GenerateImageAsync(string prompt)
    {
        var token = _configuration["Replicate:ApiToken"]
            ?? throw new InvalidOperationException("Replicate:ApiToken is not configured.");

        using var http = new HttpClient();

        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        http.DefaultRequestHeaders.Add("Prefer", "wait");

        var body = new
        {
            input = new
            {
                prompt = prompt
            }
        };

        var json = JsonSerializer.Serialize(body);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await http.PostAsync(
            "https://api.replicate.com/v1/models/black-forest-labs/flux-schnell/predictions",
            content);

        var responseJson = await response.Content.ReadAsStringAsync();
        _logger.LogInformation(responseJson ?? "MT JSON");
        if (!response.IsSuccessStatusCode)
            throw new Exception(responseJson);

        using var doc = JsonDocument.Parse(responseJson);
        var getUrl = doc.RootElement.GetProperty("urls").GetProperty("get").GetString()!;
        var status = doc.RootElement.GetProperty("status").GetString();
        var output = doc.RootElement.TryGetProperty("output", out var initialOutput) ? initialOutput.Clone() : default;
        var error = doc.RootElement.TryGetProperty("error", out var initialError) ? initialError.Clone() : default;

        // "Prefer: wait" only blocks for ~60s - if the prediction is still running after
        // that, poll the prediction URL until it reaches a terminal status.
        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (status is "starting" or "processing")
        {
            if (DateTime.UtcNow > deadline)
                throw new TimeoutException("Timed out waiting for the image generation to complete.");

            await Task.Delay(1000);

            var pollResponse = await http.GetAsync(getUrl);
            var pollJson = await pollResponse.Content.ReadAsStringAsync();
            if (!pollResponse.IsSuccessStatusCode)
                throw new Exception(pollJson);

            using var pollDoc = JsonDocument.Parse(pollJson);
            status = pollDoc.RootElement.GetProperty("status").GetString();
            output = pollDoc.RootElement.TryGetProperty("output", out var polledOutput) ? polledOutput.Clone() : default;
            error = pollDoc.RootElement.TryGetProperty("error", out var polledError) ? polledError.Clone() : default;
        }

        if (status is "failed" or "canceled")
            throw new Exception($"Image generation {status}: {(error.ValueKind == JsonValueKind.Undefined ? "unknown error" : error.ToString())}");

        if (output.ValueKind != JsonValueKind.Array || output.GetArrayLength() == 0)
            throw new Exception("Image generation succeeded but returned no output.");

        return output[0].GetString()!;
    }
}

public class ImageRequest
{
    public string Prompt { get; set; } = "";
}