using Microsoft.AspNetCore.Components;
using Rise.Client.Products.Services;
using Rise.Shared.Products;
using Rise.Client.Orders;
using Blazored.Modal.Services;

namespace Rise.Client.Products;

public partial class ProductDetail : IDisposable
{
    [Parameter, EditorRequired] public required int Id { get; set; }
    [Inject] public required IProductService ProductService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ProductenlijstStatus ProductenlijstState { get; set; }
    [Inject] public required Winkelmand Winkelmand { get; set; }

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
}