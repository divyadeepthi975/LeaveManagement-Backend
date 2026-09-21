using LeaveManagement.DTO;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginusersController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginusersController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        // Register a new login user
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDTO registerDTO)
        {
            var result = await _loginService.RegisterAsync(registerDTO);

            return Ok(result);
        }

        // Login and generate JWT token
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginDTO loginDTO)
        {
            var result = await _loginService.LoginAsync(loginDTO);

            return Ok(result);
        }
    }
}