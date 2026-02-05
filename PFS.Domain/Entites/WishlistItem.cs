using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Domain.Entites
{
    public class WishlistItem
    {
        public Guid Id { get; set; }
        public Guid WishlistId { get; set; }
        public Wishlist Wishlist { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
