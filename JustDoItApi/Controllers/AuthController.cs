using JustDoItApi.Interfaces;
using JustDoItApi.Models.Auth;
using JustDoItApi.Models.Zadachi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JustDoItApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string result = await authService.LoginAsync(model);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(new
                {
                    Status = 400,
                    IsValid = false,
                    Errors = new { Email = "Невірний логін або пароль" }
                });
            }

            return Ok(new
            {
                Token = result
            });
        }

        [HttpPost("register")]
        [Consumes("multipart/form-data")]
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

        [Authorize]
        [HttpPut("edit-profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditProfile([FromForm] EditProfileModel model)
        {
            try {
                string result = await authService.EditProfileAsync(model);
                return Ok(new
                {
                    Token = result
                });
            }
            catch(Exception e) { 
                return BadRequest(new
                {
                    Status = 400,
                    IsValid = false,
                    Errors = new { Email = e.Message }
                });
            }
        }
    }
}
