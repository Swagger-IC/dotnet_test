using Microsoft.AspNetCore.Components;
using Rise.Shared.Products;

namespace Rise.Client.Products.Components;

public partial class ProductRij
{
    [Parameter] 
    public required ProductDto product { get; set; }
    [Parameter] 
    public EventCallback OnClick { get; set; }
    private Task HandleClick() => OnClick.InvokeAsync();
}
