using System.Net.Http.Json;
using Rise.Shared.Orders;

namespace Rise.Client.Orders
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient httpClient;

        public OrderService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> CreateOrderAsync(CreateOrderDto order)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("order", order);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het plaatsen van de bestelling: {ex.Message}");
                return false;
            }
        }

        public Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        
    }
}
