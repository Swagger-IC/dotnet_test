using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;
using Rise.Shared.Products;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Rise.Shared.Users;
using System.Net.Http.Json;

public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;

    public ProductControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _httpClient = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Get_ReturnsProducts()
    {
        var response = await _httpClient.GetAsync("/api/product");

        response.EnsureSuccessStatusCode(); // Status code 200-299

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        Assert.Contains("Product 1", responseBody);
        Assert.Contains("Product 2", responseBody);
    }

    [Fact]
    public async Task GetReusableProductsOnPageOneAmountOne_ReturnsOnlyReusableProducts()
    {
        var response = await _httpClient.GetAsync("/api/product/reusable?paginanummer=1&aantal=1");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  //attributen niet hoofdlettergevoelig
        };
        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        // Kijk of alle producten op pagina 1 herbruikbaar zijn
        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is not reusable"));
        Assert.Contains(products, product => product.Name == "Reusable Product 1");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetNonReusableProductsPageTwoAmountOne_ReturnsOnlyNonReusableProducts()
    {
        var response = await _httpClient.GetAsync("/api/product/nonreusable?paginanummer=2&aantal=1");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody); 

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        // Kijk of alle producten op pagina 2 niet herbruikbaar zijn
        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Name == "Non-Reusable Product 2");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetLowStockProducts_ReturnsOnlyLowStockProducts() {
        var response = await _httpClient.GetAsync("/api/product/lowstock");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var products = JsonSerializer.Deserialize<List<ProductLeverancierDto>>(responseBody, options);

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.MinStock > product.Quantity, 
        $"Product {product.Name} does not have MinStock > Quantity (MinStock: {product.MinStock}, Quantity: {product.Quantity})"));
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnBarcodeReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?filter=1234&herbruikbaar=true&paginanummer=1&aantal=2");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is not reusable"));
        Assert.Contains(products, product => product.Barcode == "1234567890");

        Assert.Equal(1, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnBarcodeNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?filter=56&herbruikbaar=false&paginanummer=1&aantal=2");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Barcode == "5678901234");

        Assert.Equal(1, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnLokaalReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=true&paginanummer=1&aantal=2&filter=location");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is not reusable"));
        Assert.Contains(products, product => product.Location == "Location A");
        Assert.Contains(products, product => product.Location == "Location B");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnLokaalNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=false&paginanummer=1&aantal=2&filter=location%20c");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Location == "Location C");

        Assert.Equal(1, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnProductnaamReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=true&paginanummer=1&aantal=2&filter=product%201");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is non reusable"));
        Assert.Contains(products, product => product.Name == "Reusable Product 1");

        Assert.Equal(1, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnProductNaamNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=false&paginanummer=1&aantal=2&filter=prod");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Name == "Non-Reusable Product 1");
        Assert.Contains(products, product => product.Name == "Non-Reusable Product 2");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnDescriptionReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=true&paginanummer=1&aantal=2&filter=test%20reusable%20product%201");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is non reusable"));
        Assert.Contains(products, product => product.Description == "Test reusable product 1");

        Assert.Equal(1, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnDescriptionNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=false&paginanummer=1&aantal=2&filter=test%20no");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Description == "Test non-reusable product 1");
        Assert.Contains(products, product => product.Description == "Test non-reusable product 2");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnKeywordsReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=true&paginanummer=1&aantal=2&filter=a1");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is non reusable"));
        Assert.Contains(products, product => product.Keywords == "reusable, A1");

        Assert.Equal(1, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_filteredOnKeywordsNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=false&paginanummer=1&aantal=2&filter=nonreusable");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Keywords == "nonreusable, C1");
        Assert.Contains(products, product => product.Keywords == "nonreusable, D2");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_NoFiltersReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=true&paginanummer=1&aantal=2&filter=");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.True(product.Reusable, $"Product {product.Name} is non reusable"));
        Assert.Contains(products, product => product.Name == "Reusable Product 1");
        Assert.Contains(products, product => product.Name == "Reusable Product 2");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_NoFilterNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=false&paginanummer=1&aantal=2&filter=");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        Assert.All(products, product => Assert.False(product.Reusable, $"Product {product.Name} is reusable"));
        Assert.Contains(products, product => product.Name == "Non-Reusable Product 1");
        Assert.Contains(products, product => product.Name == "Non-Reusable Product 2");

        Assert.Equal(2, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_NonExistentFilterReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=true&paginanummer=1&aantal=2&filter=japajapajap");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        // Kijk of er geen producten zijn
        Assert.Empty(products);

        Assert.Equal(0, totaal);
    }

    [Fact]
    public async Task GetFilteredProducts_NonExistentFilterNonReusable()
    {
        var response = await _httpClient.GetAsync("/api/product/filtered?herbruikbaar=false&paginanummer=1&aantal=2&filter=japajapajap");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Response body in een list van ProductDto omzetten
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var pageResponse = JsonSerializer.Deserialize<PaginatieDto>(responseBody, options);
        var products = pageResponse!.Products;
        var totaal = pageResponse!.TotalCount;

        Assert.NotNull(products);

        // Kijk of er geen producten zijn
        Assert.Empty(products);

        Assert.Equal(0, totaal);
    }

    [Fact]
    public async Task IncreaseQuantity_ValidProductId_ReturnsSuccess()
    {
        int productId = 1;
        int quantityToAdd = 5;

        var content = new StringContent(quantityToAdd.ToString(), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/product/{productId}/increase", content);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
        Assert.Equal("Aantal in stock geupdate.", jsonResponse.GetProperty("message").GetString());
    
        var productResponse = await _httpClient.GetAsync($"/api/product/{productId}");
        productResponse.EnsureSuccessStatusCode();
        var productBody = await productResponse.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  // deserialization niet hoofdlettergevoelig
        };

        var product = JsonSerializer.Deserialize<ProductDto>(productBody, options);

        Assert.NotNull(product);
        Assert.Equal(15, product.Quantity); 
    }

    [Fact]
    public async Task IncreaseQuantity_InvalidQuantity_ReturnsBadRequest()
    {
        int productId = 1;
        int invalidQuantity = 0;

        var content = new StringContent(invalidQuantity.ToString(), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/product/{productId}/increase", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
        Assert.Equal("Aantal moet groter zijn dan 0. (Parameter 'quantityToAdd')", jsonResponse.GetProperty("message").GetString());
    }

    [Fact]
    [Authorize(Roles = "Administrator")]
    public async Task Create_CreatesNewUser()
    {
        // Arrange: Create a new User object
        var newUser = new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!",
            RoleId = "rol_4ofpSMHqJNRw40YZ"
        };

        // Act: Send a POST request to create the user
        var response = await _httpClient.PostAsJsonAsync("user", newUser);

        // Assert: Check if the response indicates success
        response.EnsureSuccessStatusCode(); // Expecting 201 Created

        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Optionally, parse the response if the API returns the created user
        var createdUser = JsonSerializer.Deserialize<UserDto>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(createdUser);
        Assert.Equal(newUser.Email, createdUser.Email);

    }


}




