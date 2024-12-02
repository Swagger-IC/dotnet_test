using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Orders
{
    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }
}
