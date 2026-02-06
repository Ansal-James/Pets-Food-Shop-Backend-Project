using System;
using PFS.Application.DTOs.Wishlist;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(Guid userId, Guid productId);
        Task<List<WishlistItemResponseDto>> GetWishlistAsync(Guid userId);
        Task RemoveFromWishlistAsync(Guid userId, Guid productId);
    }
}
