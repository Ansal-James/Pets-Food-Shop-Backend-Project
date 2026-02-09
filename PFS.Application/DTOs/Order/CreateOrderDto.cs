using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFS.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public string PaymentMethod { get; set; } = "DUMMY";
    }
}