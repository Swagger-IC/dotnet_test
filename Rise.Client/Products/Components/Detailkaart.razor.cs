using Microsoft.AspNetCore.Components;
using Rise.Client.Orders;
using Rise.Shared.Products;


namespace Rise.Client.Products.Components;

public partial class Detailkaart
{
    [Parameter, EditorRequired] public required ProductDto Product { get; set; }
    [Inject] public Winkelmand Winkelmand { get; set; } = default!;

    private int amount = 1;

    private void AddToCart(){
        Winkelmand.AddItem(Product.Id, Product.Name, amount, Product.Quantity);
        amount = 1;
    }
}