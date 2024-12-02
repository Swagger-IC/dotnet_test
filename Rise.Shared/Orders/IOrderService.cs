namespace Rise.Shared.Orders
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<bool> CreateOrderAsync(CreateOrderDto createOrderDto);
    }
}
