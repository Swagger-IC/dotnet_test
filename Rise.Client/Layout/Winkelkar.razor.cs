using Microsoft.AspNetCore.Components;
using Rise.Client.Orders;

namespace Rise.Client.Layout
{
    public partial class Winkelkar
    {
        [Inject] public required NavigationManager NavigationManager { get; set; }
        [Inject] public required Winkelmand Winkelmand { get; set; }

        private void WinkelOverzicht()
        {
            NavigationManager.NavigateTo("/winkelmand");
        }


        protected override void OnInitialized()
        {
            Winkelmand.Load();
            Winkelmand.OnCartChanged += StateHasChanged;

        }
    }
}