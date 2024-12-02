using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
