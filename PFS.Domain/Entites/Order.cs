using PFS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Domain.Entites
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}
