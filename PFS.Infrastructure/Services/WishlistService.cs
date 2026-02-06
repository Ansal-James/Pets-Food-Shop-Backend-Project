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
            
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
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
            }

            var alreadyAdded = wishlist.WishlistItems
                .Any(wi => wi.ProductId == productId);

            if (alreadyAdded)
                throw new AlreadyExisitException("Product already in wishlist");

            wishlist.WishlistItems.Add(new WishlistItem
            {
                Id = Guid.NewGuid(),
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
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                throw new NotFoundException("Wishlist not found");

            var item = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.ProductId == productId);

            if (item == null)
                throw new NotFoundException("Product not found in wishlist");

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
