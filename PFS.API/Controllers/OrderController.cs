using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFS.API.Helpers;
using PFS.Application.DTOs.Order;
using PFS.Application.Interface;

namespace PFS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await _orderService.CreateOrderAsync(userId, dto);
            return Ok(result);
        }

        // Get all orders of logged-in user
        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await _orderService.GetUserOrdersAsync(userId);
            return Ok(result);
        }

        // Get single order of logged-in user
        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetUserOrderById(Guid orderId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var result = await _orderService.GetUserOrderByIdAsync(userId, orderId);
            return Ok(result);
        }

        // Get all orders (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }

        // Update order status (Admin)
        //[Authorize(Roles = "Admin")]
        //[HttpPut("admin/{orderId:guid}/status")]
        //public async Task<IActionResult> UpdateOrderStatus(
        //    Guid orderId,
        //    [FromQuery] string status)
        //{
        //    await _orderService.UpdateOrderStatusAsync(orderId, status);
        //    return Ok(new { message = "Order status updated successfully" });
        //}
    }
}
