using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.API.Helpers;
using PFS.Application.Interface;
using System.Security.Claims;
using PFS.Application.Resources;
using PFS.Application.Responses;
using PFS.Application.DTOs.Wishlist;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("{productId:guid}")]
        public async Task<IActionResult> AddToWishlist(Guid productId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            await _wishlistService.AddToWishlistAsync(userId, productId);
            return Ok(ApiResponse<string>.SuccessResponse(SuccessApiMessages.AddWishList));
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await _wishlistService.GetWishlistAsync(userId);
            return Ok(ApiResponse<List<WishlistItemResponseDto>>.SuccessResponse(SuccessApiMessages.FetchWishlist,result));
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid productId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            return Ok(ApiResponse<string>.SuccessResponse(SuccessApiMessages.RemoveWishlist));
        }
    }
}
