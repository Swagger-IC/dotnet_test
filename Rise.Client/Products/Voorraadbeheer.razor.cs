using Microsoft.AspNetCore.Components;
using Rise.Shared.Products;
using Rise.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rise.Client.Products
{
    public partial class Voorraadbeheer
    {
        private IEnumerable<ProductLeverancierDto>? products;
        private bool isAscending = true;

        private Dictionary<int, BijbestelModel> bijbestelModels = new();

        [Inject]
        private IProductService ProductService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            products = await ProductService.GetLowStockProductsAsync();
            if (products != null)
            {
                foreach (var product in products)
                {
                    if (!bijbestelModels.ContainsKey(product.Id))
                    {
                        bijbestelModels[product.Id] = new BijbestelModel();
                    }
                }
            }
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

        private async Task HandleValidSubmit(int productId)
        {
            try
            {
                var model = bijbestelModels[productId];
                model.ServerError = null;

                await ProductService.IncreaseQuantityAsync(productId, model.QuantityToAdd!.Value);

                products = await ProductService.GetLowStockProductsAsync();
                model.QuantityToAdd = 0;
            }
            catch (HttpRequestException ex)
            {
                bijbestelModels[productId].ServerError = ex.Message;
                StateHasChanged();
            }
        }

        private string GetStockStatus(ProductLeverancierDto product)
        {
            return product.Quantity <= (product.MinStock / 2) ? "bg-danger" : "bg-warning";
        }
    }
}