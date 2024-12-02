using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Products;
using Rise.Domain.Products;
using FluentValidation;


namespace Rise.Services.Products;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        IQueryable<ProductDto> query = dbContext.Products.Select(static x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                Description = x.Description,
                Reusable = x.Reusable,
                Quantity = x.Quantity,
                ImgUrl = x.ImgUrl,

                Barcode = x.Barcode,
                Keywords = x.Keywords,
                MinStock = x.MinStock
            });

        var products = await query.ToListAsync();

        return products;
    }

    public async Task<(IEnumerable<ProductDto> products, int totalCount)> GetProductsByReusabilityAsync(bool isReusable, int paginanummer, int aantal)
    {
        var skip = (paginanummer - 1) * aantal;

        var products = await dbContext.Products
            .Where(x => x.Reusable == isReusable)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                Description = x.Description,
                Reusable = x.Reusable,
                Quantity = x.Quantity,
                ImgUrl = x.ImgUrl,
                Barcode = x.Barcode,
                MinStock = x.MinStock,
                Keywords = x.Keywords
            })
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(aantal)
            .ToListAsync();

        
        var totalCount = await dbContext.Products
            .Where(x => x.Reusable == isReusable)
            .CountAsync();

        return (products, totalCount);
    }


    public async Task<ProductDto> GetProductById(int id)
    {
        var product = await dbContext.Products
            .Where(x => x.Id == id)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                Description = x.Description,
                Reusable = x.Reusable,
                Quantity = x.Quantity,
                Barcode = x.Barcode,
                MinStock = x.MinStock,
                Keywords = x.Keywords,
                ImgUrl = x.ImgUrl,
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new KeyNotFoundException($"Product with Id {id} was not found.");
        }

        return product;
    }

    public async Task<(IEnumerable<ProductDto> products, int totalCount)> GetFilteredProducts(string? filter, bool herbruikbaar, int paginanummer, int aantal)
    {
        var skip = (paginanummer - 1) * aantal;

        var products = await dbContext.Products
             .Where(x => x.Reusable == herbruikbaar && (string.IsNullOrEmpty(filter) || x.Name.ToLower().Contains(filter) || x.Location.ToLower().Contains(filter) || x.Description.ToLower().Contains(filter) || x.Barcode.ToLower().Contains(filter) || x.Keywords.ToLower().Contains(filter)))

            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                Description = x.Description,
                Reusable = x.Reusable,
                Quantity = x.Quantity,
                Barcode = x.Barcode,
                MinStock = x.MinStock,
                Keywords = x.Keywords,
                ImgUrl = x.ImgUrl  
            })
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(aantal)
            .ToListAsync();

        var totalCount = await dbContext.Products
              .Where(x => x.Reusable == herbruikbaar && (string.IsNullOrEmpty(filter) || x.Name.ToLower().Contains(filter) || x.Location.ToLower().Contains(filter) || x.Description.ToLower().Contains(filter) || x.Barcode.ToLower().Contains(filter) || x.Keywords.ToLower().Contains(filter)))

            .CountAsync();

        return (products, totalCount);
    }


    public Task<(IEnumerable<ProductDto> products, int totalCount)> GetNotReusableProductsAsync(int paginanummer, int aantal)
    {
        return GetProductsByReusabilityAsync(false, paginanummer, aantal);
    }

    public Task<(IEnumerable<ProductDto> products, int totalCount)> GetReusableProductsAsync(int paginanummer, int aantal)
    {
        return GetProductsByReusabilityAsync(true, paginanummer, aantal);
    }

    public async Task<IEnumerable<ProductLeverancierDto>> GetLowStockProductsAsync()
    {
        IQueryable<ProductLeverancierDto> query = dbContext.Products
            .Include(x => x.Leverancier)
            .Where(x => x.Quantity < x.MinStock)
            .Select(x => new ProductLeverancierDto
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                Description = x.Description,
                Reusable = x.Reusable,
                Quantity = x.Quantity,
                Barcode = x.Barcode,
                MinStock = x.MinStock,
                Keywords = x.Keywords,
                LeverancierEmail = x.Leverancier.Email
            });

        var products = await query.ToListAsync();

        return products;
    }
    public async Task IncreaseQuantityAsync(int productId, int quantityToAdd)
    {
        if (quantityToAdd <= 0)
        {
            throw new ArgumentException("Aantal moet groter zijn dan 0.", nameof(quantityToAdd));
        }

        Product? product = await dbContext.Products.SingleOrDefaultAsync(x => x.Id == productId);

        if (product is null)
        {
            throw new KeyNotFoundException($"Product met Id {productId} werd niet gevonden.");
        }

        product.Quantity += quantityToAdd;

        await dbContext.SaveChangesAsync();

    }


    public async Task<bool> CreateProductAsync(CreateProductDto createDto)
    {
        
        

        var leverancier = await dbContext.Leveranciers
            .FirstOrDefaultAsync(x => x.Name == "Leverancier A") ?? throw new Exception($"Leverancier Leverancier A niet gevonden.");

        bool isReusable = createDto.Reusable == "true";

        var product = new Product
        {
            Name = createDto.Name!,
            Location = createDto.Location!,
            Description = createDto.Description!,
            Reusable = isReusable,
            Quantity = createDto.Quantity ,
            Barcode = createDto.Barcode!,
            ImgUrl = createDto.ImgUrl!,
            MinStock = createDto.MinStock,
            Keywords = createDto.Keywords!,
            Leverancier = leverancier 
        };

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public Task<HttpResponseMessage> UploadImageAsync(MultipartFormDataContent content)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductDto> GetProductByBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new ArgumentException("Barcode mag niet leeg zijn.", nameof(barcode));
        }

        var product = await dbContext.Products
            .Where(p => p.Barcode == barcode)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                Description = p.Description,
                Reusable = p.Reusable,
                Quantity = p.Quantity,
                Barcode = p.Barcode,
                MinStock = p.MinStock,
                Keywords = p.Keywords,
                ImgUrl = p.ImgUrl
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new KeyNotFoundException($"Geen product gevonden met barcode: {barcode}");
        }

        return product;
    }

    public async Task<bool> DecreaseQuantitiesAsync(Dictionary<int, int> productQuantities)
    {
        if (productQuantities == null || !productQuantities.Any())
        {
            throw new ArgumentException("De lijst met producten mag niet leeg zijn.", nameof(productQuantities));
        }

        // Haal alle betrokken producten op
        var productIds = productQuantities.Keys;
        var products = await dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        // Controleer of alle opgegeven product-IDs bestaan
        foreach (var productId in productIds)
        {
            if (!products.Any(p => p.Id == productId))
            {
                throw new KeyNotFoundException($"Product met ID {productId} is niet gevonden.");
            }
        }

        // Verwerk elke producthoeveelheid
        foreach (var (productId, amount) in productQuantities)
        {
            if (amount <= 0)
            {
                throw new ArgumentException($"De hoeveelheid voor product met ID {productId} moet groter zijn dan nul.");
            }

            var product = products.First(p => p.Id == productId);

            if (product.Quantity < amount)
            {
                throw new InvalidOperationException($"De voorraad van product met ID {productId} is onvoldoende om deze hoeveelheid te verminderen.");
            }

            // Verlaag de voorraad
            product.Quantity -= amount;
        }

        // Sla de wijzigingen op in de database
        dbContext.Products.UpdateRange(products);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product == null)
        {
            return false; // Product niet gevonden
        }

        dbContext.Products.Remove(product); // Verwijder het product
        await dbContext.SaveChangesAsync(); // Sla de wijzigingen op

        return true; // Succesvol verwijderd

    }

}
