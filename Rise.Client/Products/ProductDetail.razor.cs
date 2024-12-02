using Microsoft.AspNetCore.Components;
using Rise.Client.Products.Services;
using Rise.Shared.Products;
using Rise.Client.Orders;
using Blazored.Modal.Services;
using Rise.Shared.Orders;

namespace Rise.Client.Products;

public partial class ProductDetail : IDisposable
{
    [Parameter, EditorRequired] public required int Id { get; set; }
    [Inject] public required IProductService ProductService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ProductenlijstStatus ProductenlijstState { get; set; }
    [Inject] public required Winkelmand Winkelmand { get; set; }
    [Inject] public required IOrderService orderService { get; set; }
    [Inject] public required IProductService productService { get; set; }
    private CreateOrderDto Order { get; set; } = new CreateOrderDto();

    private ProductDto? product;
    protected override async Task OnInitializedAsync()
    {
        await GetProductById(Id);
    }
    protected override void OnInitialized()
    {
        Winkelmand.Load();
        Winkelmand.OnCartChanged += StateHasChanged;   
    }
    public void Dispose()
    {
        Winkelmand.OnCartChanged -= StateHasChanged;
    }
    private async Task GetProductById(int id)
    {
        product = await ProductService.GetProductById(id);
    }
    private void NavigateBack()
    {
        ProductenlijstState.VanDetailPagina = true;
        NavigationManager.NavigateTo("/");
    }

    private async void PlaatsBestelling()
    {
        try
        {
            Order.UserId = 1;
            Order.OrderItems = Winkelmand.Items.Select(item => new CreateOrderItemDto
            {
                ProductId = item.ProductId,
                Amount = item.Amount
            }).ToList();

            var response = await orderService.CreateOrderAsync(Order);

            if (response)
            {
                var productQuantities = Winkelmand.Items.ToDictionary(item => item.ProductId, item => item.Amount);

                var decreaseResponse = await productService.DecreaseQuantitiesAsync(productQuantities);

                if (decreaseResponse)
                {
                    Winkelmand.Clear();
                    StateHasChanged();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
        }

    }
}