using Microsoft.AspNetCore.Components;
using Rise.Client.Products.Services;

namespace Rise.Client.Orders.Components
{
    public partial class Overzichtlijn
    {
        [Parameter, EditorRequired] public required WinkelmandItem Lijn { get; set; }
        [Inject] public Winkelmand Winkelmand { get; set; } = default!;

        public void OnAmountChanged(ChangeEventArgs e)
        {
            string aantal = e.Value?.ToString() ?? string.Empty;
            if(aantal != null)
            {
                Winkelmand.AmountChanged(Lijn.ProductId, int.Parse(aantal));
            }  
        }
    }
}