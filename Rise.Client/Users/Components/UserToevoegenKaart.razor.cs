using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Roles;
using Rise.Shared.Users;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Rise.Client.Users.Components
{
    public partial class UserToevoegenKaart
    {
        [Inject] public required IUserService UserService { get; set; }
        [Inject] public required IRolService RolService { get; set; }
        [Inject] public required NavigationManager NavigationManager { get; set; }
        [Inject] public required IToastService ToastService { get; set; }

        private readonly CreateUserDto createUserDto = new();

        private IEnumerable<RolDto>? roles;
        private string emailErrorMessage = string.Empty;
        protected override async Task OnInitializedAsync()
        {

            roles = await RolService.GetRolesAsync();
        }

        private async Task UserToevoegen()
        {
            try
            {
                    emailErrorMessage = string.Empty;

                    var nieuweUser = await UserService.CreateUserAsync(createUserDto);

                    if (!string.IsNullOrWhiteSpace(nieuweUser.FirstName))
                    {
                        ToastService.ShowSuccess($"Gebruiker {nieuweUser.FirstName} {nieuweUser.LastName} succesvol aangemaakt!");
                        NavigationManager.NavigateTo("/users");
                    }
                
            }
            catch (HttpRequestException httpEx)
            {
                if (httpEx.StatusCode == HttpStatusCode.Conflict)
                {
                    
                    emailErrorMessage = "Er bestaat al een gebruiker met dit emailadres.";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception: {ex.Message}");
            }
        }

    }

}