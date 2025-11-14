using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly LoginService _loginService;
        public AuthController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequest request)
        {
            var (account, token) = await _loginService.AuthenticateAsync(request.Username, request.Password);

            if (account == null || token == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new AccountLoginResponse
            {
                Id = account.Id,
                Username = account.Username,
                Role = account.Role,
                Status = account.Status,
                Token = token
            });
        }
    }
}
