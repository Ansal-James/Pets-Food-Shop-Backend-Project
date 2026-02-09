using PFS.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.Interface
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(Guid userId, CreateOrderDto dto);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(Guid userId);
        Task<OrderResponseDto> GetUserOrderByIdAsync(Guid userId, Guid orderId);

        //Admin functionalities
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(Guid orderId, string status);
    }
}
