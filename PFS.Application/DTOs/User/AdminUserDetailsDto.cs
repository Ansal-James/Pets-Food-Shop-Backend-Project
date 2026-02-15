using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.DTOs.User
{
    public class AdminUserDetailsDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
