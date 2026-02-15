using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.Application.DTOs.Auth;
using PFS.Application.Interface;
using PFS.Application.Resources;
using PFS.Application.Responses;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        public UserController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpDto signUp)
        {
            await _authService.RegisterAsync(signUp);
            return Ok(ApiResponse<string>.SuccessResponse(SuccessApiMessages.SuccessRegistation));
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await _authService.LoginAsync(login);

            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(SuccessApiMessages.LoginSuccess, result));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(SuccessApiMessages.TokenRefresh,result));
        }
    }
}
