using JustDoItApi.Interfaces;
using JustDoItApi.Models.Auth;
using JustDoItApi.Models.Zadachi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JustDoItApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController (IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string result = await authService.LoginAsync(model);
            return Ok(new
            {
                Token = result
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            string result = await authService.RegisterAsync(model);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(new
                {
                    Status = 400,
                    IsValid = false,
                    Errors = new { Email = "Помилка реєстрації" }
                });
            }
            return Ok(new
            {
                Token = result
            });
        }
    }
}
