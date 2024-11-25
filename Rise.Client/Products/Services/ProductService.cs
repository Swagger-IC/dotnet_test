using System.Net.Http.Json;
using Rise.Shared.Products;

namespace Rise.Client.Products.Services;

public class ProductService : IProductService
{
    private readonly HttpClient httpClient;

    public ProductService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = await httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("product");
        return products!;
    }

    public async Task<ProductDto> GetProductById(int id)
    {
        var product = await httpClient.GetFromJsonAsync<ProductDto>($"product/{id}");
        return product!;
    }

    public async Task<(IEnumerable<ProductDto> products, int totalCount)> GetNotReusableProductsAsync(int paginanummer, int aantal)
    {
        var response = await httpClient.GetFromJsonAsync<PaginatieDto>($"product/nonreusable?paginanummer={paginanummer}&aantal={aantal}");
        return (response!.Products, response.TotalCount);
    }

    public async Task<(IEnumerable<ProductDto> products, int totalCount)> GetReusableProductsAsync(int paginanummer, int aantal)
    {
        var response = await httpClient.GetFromJsonAsync<PaginatieDto>($"product/reusable?paginanummer={paginanummer}&aantal={aantal}");
        return (response!.Products, response.TotalCount);
    }

    public async Task<IEnumerable<ProductLeverancierDto>> GetLowStockProductsAsync()
    {
        var products = await httpClient.GetFromJsonAsync<IEnumerable<ProductLeverancierDto>>("product/lowstock");
        return products!;
    }

    public async Task<(IEnumerable<ProductDto> products, int totalCount)> GetFilteredProducts(string? filter, bool herbruikbaar, int paginanummer,int aantal)
    {
        var response = await httpClient.GetFromJsonAsync<PaginatieDto>($"product/filtered?herbruikbaar={herbruikbaar}&paginanummer={paginanummer}&aantal={aantal}" +
            (!string.IsNullOrEmpty(filter) ? $"&filter={Uri.EscapeDataString(filter)}" : ""));
        return (response!.Products, response.TotalCount);
    }


    public async Task IncreaseQuantityAsync(int productId, int quantityToAdd) {
        var response = await httpClient.PutAsJsonAsync($"product/{productId}/increase", quantityToAdd);
    
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(message);
        }
    } 

    public async Task<bool> CreateProductAsync(CreateProductDto createDto)
    {
        var response = await httpClient.PostAsJsonAsync("product", createDto);

        if (response.IsSuccessStatusCode)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    public async Task<HttpResponseMessage> UploadImageAsync(MultipartFormDataContent content)
    {
        // Hier gebruik je je HttpClient om de afbeelding te uploaden
        return await httpClient.PostAsync("product/upload", content);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"product/{id}");

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        return false;
    }

}


