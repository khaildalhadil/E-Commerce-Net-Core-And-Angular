using Application.Dtos.products;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BuggyController(ILogger<BuggyController> logger) : ControllerBase
{

    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        // Denied access is a security-relevant event: warning level, with the caller's IP.
        logger.LogWarning(
            "Unauthorized access to {RequestPath} from {RemoteIP}",
            HttpContext.Request.Path,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

        return Unauthorized();
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("Not a good request");
    }

    [HttpGet("notfound")]
    public IActionResult GetNotGoundd()
    {
        return NotFound();
    }

    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        // Not logged here on purpose - ExceptionMiddlware logs every unhandled exception once.
        throw new Exception("this is a test exception");
    }

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDtos createProductDtos)
    {
        return Ok();
    }

}
