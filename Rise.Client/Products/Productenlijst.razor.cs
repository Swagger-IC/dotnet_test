using Microsoft.AspNetCore.Components;
using Rise.Client.Products.Services;
using Rise.Shared.Products;

namespace Rise.Client.Products;

public partial class Productenlijst
{
    private IEnumerable<ProductDto>? products;
    [Parameter] public EventCallback OnClick { get; set; }
    [Inject] public required IProductService ProductService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ProductenlijstStatus ProductenlijstStatus { get; set; }
    private CancellationTokenSource? debounceCts;

    protected override async Task OnInitializedAsync()
    {
        if(ProductenlijstStatus.Filter != string.Empty){
            await LoadProductsAsync();
        }
        else if (ProductenlijstStatus.UitScannen){
            await LoadNonReusableProductsAsync();
        } else {
            await LoadReusableProductsAsync();
        }
        ProductenlijstStatus.VanDetailPagina = false;
    }

    private async Task LoadProductsAsync()
    {
        var response = await ProductService.GetFilteredProducts(ProductenlijstStatus.Filter, !ProductenlijstStatus.UitScannen, ProductenlijstStatus.Paginanummer, ProductenlijstStatus.Aantal);

        products = response.products;
  
        ProductenlijstStatus.TotalCount = response.totalCount;
        ProductenlijstStatus.TotalPages = (int)Math.Ceiling((double)ProductenlijstStatus.TotalCount / ProductenlijstStatus.Aantal);
    }

    private async Task LoadNonReusableProductsAsync()
    {
        ProductenlijstStatus.UitScannen = true;
        if(ProductenlijstStatus.VanDetailPagina == false){
            ProductenlijstStatus.Paginanummer = 1;
        }
        await LoadProductsAsync();
    }

    private async Task LoadReusableProductsAsync()
    {
        ProductenlijstStatus.UitScannen = false;
        if(ProductenlijstStatus.VanDetailPagina == false){
            ProductenlijstStatus.Paginanummer = 1;
        }
        await LoadProductsAsync();
    }

    private void OnProductClick(int id)
    {
        NavigationManager.NavigateTo($"product/{id}");
    }

    private async Task LoadPageData(int page)
    {
        ProductenlijstStatus.Paginanummer = page;
        await LoadProductsAsync();
    }

    private async Task LoadPageDataWithNewCount(int itemsPerPage)
    {
        ProductenlijstStatus.Aantal = itemsPerPage;
        ProductenlijstStatus.Paginanummer = 1;
        await LoadProductsAsync();
    }
    
    private async void OnFilterInput(ChangeEventArgs e)
    {
        ProductenlijstStatus.Filter = e.Value?.ToString() ?? string.Empty;

        debounceCts?.Cancel();
        debounceCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(600, debounceCts.Token);
            await HandleSearch();
        }
        catch (TaskCanceledException)
        {
            // Previous search was canceled; do nothing
        }
    }
    private async Task HandleSearch()
    {
        ProductenlijstStatus.Paginanummer = 1;
        await LoadProductsAsync();
        StateHasChanged(); // een rerender omdat product niet direct wil herladen
    }
    // Methods to get the active class
    private string GetButton1Class()
    {
        return ProductenlijstStatus.UitScannen ? "active" : string.Empty;
    }

    private string GetButton2Class()
    {
        return ProductenlijstStatus.UitScannen ? string.Empty : "active";
    }

    private string GetViewType()
    {
        return ProductenlijstStatus.TableView ? "Kaarten" : "Tabel";
    }

    private void ToggleView()
    {
        ProductenlijstStatus.TableView = !ProductenlijstStatus.TableView;
    }

    // Hiermee krijgen we terug welk icoontje we gebruiken
    private string GetToggleIconClass()
    {
        return ProductenlijstStatus.TableView ? "bi bi-grid-3x2-gap" : "bi bi-list";
    }

    private void AddProduct()
    {
        NavigationManager.NavigateTo("/add-product");
    }

    private async Task RefreshProductList()
    {
        if (ProductenlijstStatus.Filter != string.Empty)
        {
            await LoadProductsAsync(); // Als er een filter is, laad je de gefilterde producten
        }
        else if (ProductenlijstStatus.UitScannen)
        {
            await LoadNonReusableProductsAsync(); // Laad producten die niet uitleenbaar zijn
        }
        else
        {
            await LoadReusableProductsAsync(); // Laad producten die uitleenbaar zijn
        }
    }
}
