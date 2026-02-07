using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.DTOs.Cart
{
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
        public decimal GrandTotal => Items.Sum(i => i.TotalPrice);
    }
}
