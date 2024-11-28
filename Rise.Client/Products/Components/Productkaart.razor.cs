using Microsoft.AspNetCore.Components;

namespace Rise.Client.Products.Components;

public partial class Productkaart
{
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
}
