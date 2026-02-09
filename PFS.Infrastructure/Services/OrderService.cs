using Microsoft.EntityFrameworkCore;
using PFS.Application.DTOs.Order;
using PFS.Application.Execeptions;
using PFS.Application.Interface;
using PFS.Domain.Entites;
using PFS.Domain.Enums;
using PFS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;
        public OrderService(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }
        public async Task<OrderResponseDto> CreateOrderAsync(Guid userId, CreateOrderDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new BadRequestException("Cart is empty");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;

            foreach (var cartItem in cart.CartItems)
            {
                if (!cartItem.Product.IsActive)
                    throw new BadRequestException("One or more products are not available");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = cartItem.Product.Id,
                    ProductName = cartItem.Product.Name,
                    Price = cartItem.Product.Price,
                    Quantity = cartItem.Quantity
                };

                totalAmount += orderItem.Price * orderItem.Quantity;
                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            // Dummy payment 
            order.Status = OrderStatus.Confirmed;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear cart after order
            await _cartService.ClearCartAsync(userId);

            return MapToResponseDto(order);
        }
        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(Guid userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToResponseDto).ToList();
        }
        public async Task<OrderResponseDto> GetUserOrderByIdAsync(Guid userId, Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new NotFoundException("Order not found");

            return MapToResponseDto(order);
        }
        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToResponseDto).ToList();
        }
        public async Task UpdateOrderStatusAsync(Guid orderId, string status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException("Order not found");

            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                throw new BadRequestException("Invalid order status");

            order.Status = orderStatus;
            await _context.SaveChangesAsync();
        }

        private static OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Price * oi.Quantity
                }).ToList()
            };
        }
    }
}
