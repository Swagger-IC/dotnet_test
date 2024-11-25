using Microsoft.AspNetCore.Components;
using Rise.Shared.Products;

namespace Rise.Client.Products
{
    public partial class ProductToevoegen
    {
        private async Task AddProduct(CreateProductDto createDto)
        {
            try
            {
                var newProduct = await ProductService.CreateProductAsync(createDto);
                NavigationManager.NavigateTo("/"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}