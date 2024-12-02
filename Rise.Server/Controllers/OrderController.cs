using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Orders;


namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId(int userId)
        {
            try
            {
                var orders = await orderService.GetOrdersByUserIdAsync(userId);
                Console.WriteLine(orders);

                if (orders == null || !orders.Any())
                {
                    return NotFound($"Geen bestellingen gevonden voor gebruiker met ID: {userId}");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Er is een fout opgetreden bij het ophalen van de bestellingen: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var result = await orderService.CreateOrderAsync(createOrderDto);

                if (result)
                {
                    return Ok(true); 
                }

                return BadRequest("Het aanmaken van de bestelling is mislukt."); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Er is een fout opgetreden bij het maken van de bestelling: {ex.Message}");
            }
        }
    }
}
