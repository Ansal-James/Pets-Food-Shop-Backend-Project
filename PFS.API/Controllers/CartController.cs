using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.API.Helpers;
using PFS.Application.DTOs.Cart;
using PFS.Application.Interface;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            await _cartService.AddToCartAsync(userId, dto);
            return Ok(new { message = "Product added to cart" });
        }
        [HttpPut("{cartItemId:guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            await _cartService.UpdateCartItemAsync(userId, cartItemId, dto);
            return Ok(new { message = "Cart item updated successfully" });
        }
        [HttpDelete("{cartItemId:guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            await _cartService.RemoveCartItemAsync(userId, cartItemId);
            return Ok(new { message = "Cart item removed successfully" });
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await _cartService.GetCartAsync(userId);
            return Ok(result);
        }

    }
}
