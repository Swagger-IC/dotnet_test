using Microsoft.AspNetCore.Components;
using Rise.Shared.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Client.Products;

public partial class Voorraadbeheer
{
    private IEnumerable<ProductLeverancierDto>? products;
    private bool isAscending = true;

    [Inject]
    private IProductService ProductService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetLowStockProductsAsync();
    }

    private void SortByQuantity()
    {
        if (products != null)
        {
            products = isAscending
                ? products.OrderBy(p => p.Quantity).ToList() 
                : products.OrderByDescending(p => p.Quantity).ToList(); 

            isAscending = !isAscending;
        }
    }

    private string GetSortIcon()
    {
        return isAscending ? "▲" : "▼";
    }

    private void OnOrderClick(int productId)
    {
        Console.WriteLine($"Bestel button clicked for product Id: {productId}");
    }

    private string GetStockStatus(ProductLeverancierDto product)
    {
        return product.Quantity <= (product.MinStock / 2) ? "bg-danger" : "bg-warning";
    }
}

