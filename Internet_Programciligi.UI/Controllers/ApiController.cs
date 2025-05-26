using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Internet_Programciligi.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("SetToken")]
        public IActionResult SetToken([FromBody] TokenModel model)
        {
            if (string.IsNullOrEmpty(model.Token))
            {
                return BadRequest("Token is required");
            }

            HttpContext.Session.SetString("JWTToken", model.Token);
            return Ok(new { success = true });
        }

        [HttpGet("GetToken")]
        public IActionResult GetToken()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            return Ok(new { token = token });
        }
    }

    public class TokenModel
    {
        public string Token { get; set; }
    }
} 