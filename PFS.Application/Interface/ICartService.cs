using PFS.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid userId, AddToCartDto dto);
        Task UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto);
        Task RemoveCartItemAsync(Guid userId, Guid cartItemId);
        Task<CartResponseDto> GetCartAsync(Guid userId);
        Task ClearCartAsync(Guid userId);
    }
}
