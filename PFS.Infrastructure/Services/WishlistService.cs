using Microsoft.EntityFrameworkCore;
using PFS.Application.DTOs.Wishlist;
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
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;
        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToWishlistAsync(Guid userId, Guid productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

            if (product == null)
                throw new NotFoundException("Product not found");

            // Get or create wishlist (NO Include)
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync(); // ensure WishlistId
            }

            //  ALWAYS query WishlistItems directly
            var alreadyAdded = await _context.WishlistItems
                .AnyAsync(wi =>
                    wi.WishlistId == wishlist.Id &&
                    wi.ProductId == productId);

            if (alreadyAdded)
                throw new AlreadyExisitException("Product already in wishlist");

            _context.WishlistItems.Add(new WishlistItem
            {
                Id = Guid.NewGuid(),
                WishlistId = wishlist.Id,
                ProductId = productId
            });

            await _context.SaveChangesAsync();
        }


        public async Task<List<WishlistItemResponseDto>> GetWishlistAsync(Guid userId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                return new List<WishlistItemResponseDto>();

            return wishlist.WishlistItems
                .Where(wi => wi.Product.IsActive)
                .Select(wi => new WishlistItemResponseDto
                {
                    WishlistItemId = wi.Id,
                    ProductId = wi.Product.Id,
                    ProductName = wi.Product.Name,
                    ProductImageUrl = wi.Product.ImageUrl,
                    Price = wi.Product.Price
                }).ToList();
        }

        public async Task RemoveFromWishlistAsync(Guid userId, Guid productId)
        {
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(wi =>
                    wi.ProductId == productId &&
                    _context.Wishlists.Any(w =>
                        w.Id == wi.WishlistId &&
                        w.UserId == userId));

            if (wishlistItem == null)
                throw new NotFoundException("Product not found in wishlist");

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            //  Important for delete → add scenarios
            _context.ChangeTracker.Clear();
        }

    }
}
