using LeaveManagement.DTO;
using LeaveManagement.Services;

using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeDTO registerDTO)
        {
            var result =
                await _loginService.RegisterEmployeeAsync(
                    registerDTO);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result =
                await _loginService.LoginAsync(loginDTO);

            return Ok(result);
        }
    }
}