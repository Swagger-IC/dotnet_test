using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Products;

namespace Rise.Client.Products.Components;

public partial class Productkaart
{
    [Parameter] public EventCallback OnProductDeleted { get; set; }

    [Inject] public required IProductService ProductService { get; set; }
    [Inject] public required IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public required int ProductId { get; set; }
    [Parameter]
    public required string ImageUrl { get; set; }
    [Parameter]
    public required string Naam { get; set; }
    [Parameter]
    public required string Lokaal { get; set; }
    [Parameter] 
    public EventCallback OnClick { get; set; }
    private Task HandleClick() => OnClick.InvokeAsync();

    private async Task DeleteProduct()
    {
        // Bevestiging van verwijderen met JSRuntime en object[] voor de parameters
        var confirm = await JSRuntime.InvokeAsync<bool>("confirm", new object[] { $"Weet je zeker dat je {Naam} wilt verwijderen?" });

        if (confirm)
        {
            var success = await ProductService.DeleteProductAsync(ProductId); // Verwijder het product
            if (success)
            {
                await JSRuntime.InvokeAsync<Task>("alert", new object[] { "Product succesvol verwijderd." });
                // Update de UI of navigeer naar een andere pagina na succesvolle verwijdering
                await OnProductDeleted.InvokeAsync();
            }
            else
            {
                await JSRuntime.InvokeAsync<Task>("alert", new object[] { "Er is een fout opgetreden bij het verwijderen van het product." });
            }
        }
    }
}
