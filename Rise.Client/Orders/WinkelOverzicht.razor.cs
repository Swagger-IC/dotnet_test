using Rise.Client.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.Orders;
using Rise.Shared.Products;
using Rise.Shared.Users;

namespace Rise.Client.Orders
{
    public partial class WinkelOverzicht
    {
        [Inject] public required Winkelmand Winkelmand { get; set; }
        [Inject] public required IOrderService orderService { get; set; }
        [Inject] public required IProductService productService { get; set; }
        [Inject] public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }


        private CreateOrderDto Order { get; set; } = new CreateOrderDto();

        private bool bestellingSuccesvol = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            Order.UserId = int.Parse(user.GetUserId());

            Winkelmand.Load();
            Winkelmand.OnCartChanged += StateHasChanged;
        }

        private async void PlaatsBestelling()
        {
            try
            {
                
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
                        bestellingSuccesvol = true;
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
}