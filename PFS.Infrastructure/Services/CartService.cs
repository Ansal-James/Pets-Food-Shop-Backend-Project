using Microsoft.EntityFrameworkCore;
using PFS.Application.DTOs.Cart;
using PFS.Application.Execeptions;
using PFS.Application.Interface;
using PFS.Domain.Entites;
using PFS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CartResponseDto> GetCartAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new CartResponseDto
                {
                    CartId = Guid.Empty,
                    Items = new List<CartItemResponseDto>()
                };
            }

            var items = cart.CartItems
                .Where(ci => ci.Product.IsActive)
                .Select(ci => new CartItemResponseDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.Product.Id,
                    ProductName = ci.Product.Name,
                    ProductImageUrl = ci.Product.ImageUrl,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity
                }).ToList();

            return new CartResponseDto
            {
                CartId = cart.Id,
                Items = items
            };
        }

        public async Task AddToCartAsync(Guid userId, AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);

            if (product == null)
                throw new NotFoundException("Product not found");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            if (cartItem != null)
            {
                // Increase quantity
                cartItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = dto.ProductId,
                    CartId = cart.Id,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }
        public async Task UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto)
        {
            if (dto.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new NotFoundException("Cart not found");

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.Id == cartItemId);

            if (item == null)
                throw new NotFoundException("Cart item not found");

            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();
        }
        public async Task RemoveCartItemAsync(Guid userId, Guid cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new NotFoundException("Cart not found");

            var item = cart.CartItems
                .FirstOrDefault(ci => ci.Id == cartItemId);

            if (item == null)
                throw new NotFoundException("Cart item not found");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return;

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
        }
    }
}
