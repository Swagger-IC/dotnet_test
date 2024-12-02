using Rise.Persistence;
using Rise.Shared.Orders;
using Rise.Domain.Orders;
using Microsoft.EntityFrameworkCore;


namespace Rise.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await dbContext.Orders
                .Include(order => order.User)
                .Include(order => order.OrderItems)
                    .ThenInclude(item => item.Product)
                .Where(order => order.User.Id == userId)  
                .ToListAsync();
                
            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.User.Id,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.Product.Id,
                    ProductName = item.Product.Name,
                    Amount = item.Amount
                }).ToList()
            }).ToList();

            return orderDtos;
        }

        public async Task<bool> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == createOrderDto.UserId)
                ?? throw new Exception($"Gebruiker met ID {createOrderDto.UserId} niet gevonden.");

            var order = new Order
            {
                User = user, 
                OrderItems = new List<OrderItem>()
            };

            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                var product = await dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == orderItemDto.ProductId)
                    ?? throw new Exception($"Product met ID {orderItemDto.ProductId} niet gevonden.");

                var orderItem = new OrderItem
                {
                    Product = product, 
                    Amount = orderItemDto.Amount
                };

                order.OrderItems.Add(orderItem);
            }

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
