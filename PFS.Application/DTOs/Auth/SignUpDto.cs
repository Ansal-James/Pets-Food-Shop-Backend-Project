using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.DTOs.Auth
{
    public class SignUpDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
