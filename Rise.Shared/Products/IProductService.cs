namespace Rise.Shared.Products;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
    Task<(IEnumerable<ProductDto> products, int totalCount)> GetNotReusableProductsAsync(int paginanummer, int aantal);
    Task<(IEnumerable<ProductDto> products, int totalCount)> GetReusableProductsAsync(int paginanummer, int aantal);
    Task<(IEnumerable<ProductDto> products, int totalCount)> GetFilteredProducts(string? filter, bool herbruikbaar, int paginanummer, int aantal);
    Task<ProductDto> GetProductById(int productId);
    Task<IEnumerable<ProductLeverancierDto>> GetLowStockProductsAsync();
    Task<bool> CreateProductAsync(CreateProductDto createDto);

    Task<HttpResponseMessage> UploadImageAsync(MultipartFormDataContent content);

}