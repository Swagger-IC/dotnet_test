using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Client.Auth;

namespace Rise.Client.Auth;


public partial class LoginDisplay
{
    public required string Naam {get; set;}
    public required string Foto {get; set;}

    [Inject] public required AuthenticationStateProvider AuthenticationStateProvider {get; set;}
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        Naam = user.GetFullname();
        Foto = user.GetPicture();
    }
    
    [Inject] public required NavigationManager Navigation { get; set; }
    public void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
    
}
