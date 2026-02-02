using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.Application.DTOs.Auth;
using PFS.Application.Interface;
using PFS.Infrastructure.Services;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> login(LoginDto loginDto)
        {
            var result = await _authService.AdminLoginAsync(loginDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("test-admin")]
        public IActionResult TestAdmin()
        {
            return Ok("You are authenticated as ADMIN!");
        }
    }
}
