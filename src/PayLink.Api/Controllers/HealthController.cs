using Microsoft.AspNetCore.Mvc;

namespace PayLink.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    /// <summary>Liveness probe used by Docker, compose, and the team's smoke tests.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        service = "paylink-api",
        utc = DateTimeOffset.UtcNow,
    });
}
