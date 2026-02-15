using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.Application.Interface;
using PFS.Application.Resources;
using PFS.Application.Responses;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminUserService.GetAllUsersAsync();
            return Ok(ApiResponse<object>.SuccessResponse(SuccessApiMessages.UsersFetched, users));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _adminUserService.GetUserByIdAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(SuccessApiMessages.UserFetched, user));
        }

        [HttpPut("{id:guid}/block")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            await _adminUserService.BlockUserAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(SuccessApiMessages.UserBlocked));
        }

        [HttpPut("{id:guid}/unblock")]
        public async Task<IActionResult> UnblockUser(Guid id)
        {
            await _adminUserService.UnblockUserAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(SuccessApiMessages.UserUnbloked));
        }
    }
}
