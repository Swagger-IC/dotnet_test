using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rise.Shared.Products;

namespace Rise.Client.Products;

public class FakeProductService : IProductService
{
    private readonly List<ProductDto> _products;
    private readonly List<ProductLeverancierDto> _productLeverancier;

    public FakeProductService()
    {
        // 10 producten: 3 zijn herbruikbaar, 7 niet
        _products = Enumerable.Range(1, 10).Select(i => new ProductDto
        {
            Id = i,
            Name = $"Product {i}",
            Location = $"Location {i}",
            Description = $"Description for Product {i}",
            Reusable = i <= 3,
            Quantity = i * 10,
            Barcode = $"1234567890{i}",
            Keywords = "keyword1, keyword2",
            MinStock = 5,
            ImgUrl = "test.jpg"
        }).ToList();

        _productLeverancier = Enumerable.Range(1, 5).Select(i => new ProductLeverancierDto
        {
            Id = i,
            Name = $"Leverancier Product {i}",
            Location = $"Warehouse {i}",
            Description = $"Description for Leverancier Product {i}",
            Reusable = i % 2 == 0,
            Quantity = i * 2,
            Barcode = $"1234567890{100 + i}",
            MinStock = 5,
            Keywords = "keyword1, keyword2",
            LeverancierEmail = $"supplier{i}@example.com"
        }).ToList();
    }

    public Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        return Task.FromResult(_products.AsEnumerable());
    }

    public Task<(IEnumerable<ProductDto> products, int totalCount)> GetProductsByReusabilityAsync(bool isReusable, int paginanummer, int aantal)
    {
        var skip = (paginanummer - 1) * aantal;

        // Log het aantal producten voordat we filteren
        Console.WriteLine($"Total products: {_products.Count}");
        var products = _products.Where(x => x.Reusable == isReusable)
            .Skip(skip)
            .Take(aantal)
            .ToList();

        Console.WriteLine($"Filtered products: {products.Count}"); // Aantal na filteren

        var totalCount = _products.Count(x => x.Reusable == isReusable);
        return Task.FromResult((products.AsEnumerable(), totalCount));
    }

    public Task<ProductDto> GetProductById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Id {id} was not found.");
        }
        return Task.FromResult(product);
    }

    public Task<(IEnumerable<ProductDto> products, int totalCount)> GetFilteredProducts(string? filter, bool herbruikbaar, int paginanummer, int aantal)
    {
        var skip = (paginanummer - 1) * aantal;

        // Log de filterwaarde en paginering
        Console.WriteLine($"Filter: {filter}, Herbruikbaar: {herbruikbaar}, Page: {paginanummer}, Items per page: {aantal}");

        var products = _products.Where(x => x.Reusable == herbruikbaar &&
            (string.IsNullOrEmpty(filter) ||
            x.Name.ToLower().Contains(filter.ToLower()) ||
            x.Location.ToLower().Contains(filter.ToLower()) ||
            x.Description.ToLower().Contains(filter.ToLower()) ||
            x.Barcode.ToLower().Contains(filter.ToLower()) ||
            x.Keywords.ToLower().Contains(filter.ToLower())))
            .Skip(skip)
            .Take(aantal)
            .ToList();

        // Log het aantal producten na filtering
        Console.WriteLine($"Filtered products: {products.Count}");

        var totalCount = _products.Count(x => x.Reusable == herbruikbaar &&
            (string.IsNullOrEmpty(filter) ||
            x.Name.ToLower().Contains(filter.ToLower()) ||
            x.Location.ToLower().Contains(filter.ToLower()) ||
            x.Description.ToLower().Contains(filter.ToLower()) ||
            x.Barcode.ToLower().Contains(filter.ToLower()) ||
            x.Keywords.ToLower().Contains(filter.ToLower())));

        return Task.FromResult((products.AsEnumerable(), totalCount));
    }

    public Task<(IEnumerable<ProductDto> products, int totalCount)> GetNotReusableProductsAsync(int paginanummer, int aantal)
    {
        return GetProductsByReusabilityAsync(false, paginanummer, aantal);
    }

    public Task<(IEnumerable<ProductDto> products, int totalCount)> GetReusableProductsAsync(int paginanummer, int aantal)
    {
        return GetProductsByReusabilityAsync(true, paginanummer, aantal);
    }

    public Task<IEnumerable<ProductLeverancierDto>> GetLowStockProductsAsync()
    {
        var products = _productLeverancier.Where(x => x.Quantity < x.MinStock).ToList();
        return Task.FromResult(products.AsEnumerable());
    }

    public Task<bool> CreateProductAsync(CreateProductDto createDto)
    {
        var leverancier = _productLeverancier.FirstOrDefault(l => l.Name == "Leverancier A");
        if (leverancier == null)
        {
            var i = 1;
            leverancier = new ProductLeverancierDto
            {
                Id = i,
                Name = $"Leverancier Product {i}",
                Location = $"Warehouse {i}",
                Description = $"Description for Leverancier Product {i}",
                Reusable = i % 2 == 0,
                Quantity = i * 2,
                Barcode = $"1234567890{100 + i}",
                MinStock = 5,
                Keywords = "keyword1, keyword2",
                LeverancierEmail = $"supplier{i}@example.com"
            };
            _productLeverancier.Add(leverancier);
        }

        var newProduct = new ProductDto
        {
            Id = _products.Max(p => p.Id) + 1, 
            Name = createDto.Name,
            Location = createDto.Location,
            Description = createDto.Description,
            Reusable = createDto.Reusable == "true",
            Quantity = createDto.Quantity,
            Barcode = createDto.Barcode,
            ImgUrl = createDto.ImgUrl,
            MinStock = createDto.MinStock,
            Keywords = createDto.Keywords
        };

        _products.Add(newProduct); 
        return Task.FromResult(true);
    }

    public Task<HttpResponseMessage> UploadImageAsync(MultipartFormDataContent content)
    {
        throw new NotImplementedException();
    }
}