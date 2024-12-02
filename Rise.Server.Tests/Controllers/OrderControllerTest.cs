using Rise.Shared.Orders;
using System.Net.Http.Json;
using System.Net;
using Xunit.Abstractions;

public class OrderControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;

    public OrderControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _httpClient = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task GetOrdersByUserId_ReturnsOrders()
    {
        int userId = 1;

        var response = await _httpClient.GetAsync($"/api/order/user/{userId}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        Assert.Contains("Reusable Product 1", responseBody); 
        Assert.Contains("Non-Reusable Product 1", responseBody); 
    }

    [Fact]
    public async Task GetOrdersByUserId_ReturnsNotFound_WhenNoOrdersExist()
    {
        int userId = 9999;

        var response = await _httpClient.GetAsync($"/api/order/user/{userId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        Assert.Contains("Geen bestellingen gevonden", responseBody);
    }

    [Fact]
    public async Task CreateOrder_ReturnsOk_WhenOrderIsCreated()
    {
        var createOrderDto = new CreateOrderDto
        {
            UserId = 1, 
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto
                {
                    ProductId = 1, 
                    Amount = 2    
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/api/order", createOrderDto);

        response.EnsureSuccessStatusCode(); 

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        Assert.Contains("true", responseBody);
    }

    [Fact]
    public async Task CreateOrder_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        var createOrderDto = new CreateOrderDto
        {
            UserId = 1, 
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto
                {
                    ProductId = 9999, 
                    Amount = 1
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/api/order", createOrderDto);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        Assert.Contains("Er is een fout opgetreden bij het maken van de bestelling", responseBody); 
    }
}
