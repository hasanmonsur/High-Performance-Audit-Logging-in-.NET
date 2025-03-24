using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditLoggingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Audit logging test endpoint");
        }

        [HttpPost]
        public IActionResult Post([FromBody] string data)
        {
            return Ok($"Received: {data}");
        }
    }
}
