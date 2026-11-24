using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PayLink.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = 200,
                service = "PayLink APP",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
