using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Products;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService productService;
    private MinioService minioService = new MinioService();

    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductDto>> Get()
    {
        var products = await productService.GetProductsAsync();
        return products;
    }

    [HttpGet("nonreusable")]
    public async Task<IActionResult> GetNonReusableProducts([FromQuery] int paginanummer, [FromQuery] int aantal)
    {
        var (products, totalCount) = await productService.GetNotReusableProductsAsync(paginanummer, aantal);
        return Ok(new { products, totalCount });
    }

    [HttpGet("reusable")]
    public async Task<IActionResult> GetReusableProducts([FromQuery] int paginanummer, [FromQuery] int aantal)
    {
        var (products, totalCount) = await productService.GetReusableProductsAsync(paginanummer, aantal);
        return Ok(new { products, totalCount });
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await productService.GetProductById(id);
        if (product == null)
            return NotFound();
        return product;
    }

    [HttpGet("lowstock")]
    public async Task<IEnumerable<ProductLeverancierDto>> GetLowStockProducts()
    {
        var products = await productService.GetLowStockProductsAsync();
        return products;
    }

    [HttpGet("filtered")]
    public async Task<ActionResult> GetFilteredProducts(
     [FromQuery] string? filter, [FromQuery]bool herbruikbaar, [FromQuery] int paginanummer, [FromQuery] int aantal)
    {
        var (products, totalCount) = await productService.GetFilteredProducts(filter, herbruikbaar, paginanummer, aantal);
        return Ok(new { products, totalCount });
    }

    [HttpPut("{productId}/increase")]
    // [Authorize(Roles = "VoorraadBeheerder")]
    public async Task<IActionResult> IncreaseQuantity(int productId, [FromBody] int quantityToAdd)
    {
        Console.WriteLine($"Aantal in stock voor product met id {productId} verhogen met {quantityToAdd}...");

        try
        {
            await productService.IncreaseQuantityAsync(productId, quantityToAdd);
            Console.WriteLine($"Aantal in stock geupdate");
            return Ok(new { Message = "Aantal in stock geupdate." });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Meegegeven aantal is niet geldig");
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Product met id {productId} niet gevonden");
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error tijdens toevoegen aan aantal in stock", Details = ex.Message });
        }
    }
    [HttpPost]
    // [Authorize(Roles = "VoorraadBeheerder")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createDto)
    {
        if (createDto == null)
        {
            return BadRequest("Product gegevens zijn verplicht.");
        }

        try
        {
            var success = await productService.CreateProductAsync(createDto);
            return success ? Ok(success) : BadRequest();
            
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is iets misgegaan bij het aanmaken van het product: {ex.Message}");
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("Geen afbeelding geselecteerd.");
        }

        try
        {
            await using var stream = image.OpenReadStream();
            var imageUrl = await minioService.UploadImageAsync("products", stream);

            // Retourneer de URL van de afbeelding na het uploaden
            return Ok(imageUrl);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is een fout opgetreden bij het uploaden van de afbeelding: {ex.Message}");
        }
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return BadRequest("Barcode mag niet leeg zijn.");
        }

        try
        {
            var product = await productService.GetProductByBarcodeAsync(barcode);
            if (product == null)
            {
                return NotFound($"Geen product gevonden met barcode: {barcode}");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is een fout opgetreden bij het ophalen van het product: {ex.Message}");
        }
    }

    [HttpPost("decreaseQuantities")]
    public async Task<IActionResult> DecreaseProductQuantities([FromBody] Dictionary<int, int> productQuantities)
    {
        if (productQuantities == null || !productQuantities.Any())
        {
            return BadRequest("De lijst met producten mag niet leeg zijn.");
        }

        try
        {
            // Verlaag de hoeveelheden via de service
            await productService.DecreaseQuantitiesAsync(productQuantities);

            return Ok("De hoeveelheden van de opgegeven producten zijn succesvol verlaagd.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var success = await productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound("Product niet gevonden.");
            }

            return NoContent(); // 204 StatusCode, betekent dat de verwijdering succesvol was maar er geen content wordt geretourneerd.
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is iets misgegaan bij het verwijderen van het product: {ex.Message}");
        }
    }


}
