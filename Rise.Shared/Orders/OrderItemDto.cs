using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Orders
{
    public class OrderItemDto
    {
        public required int ProductId { get; set; } // Het ID van het product in het orderitem
        public required string ProductName { get; set; }
        public required int Amount { get; set; }
    }
}
