using Rise.Shared.Models;
using Rise.Shared.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Client.Auth;

namespace Rise.Client.Users;

public partial class Profiel
{
    [Parameter]  public required string Voornaam {get;set;}
    [Parameter]  public required string Rol {get;set;}
    [Parameter]  public required string Naam {get;set;}
    [Parameter]  public required string Email {get;set;}
    [Parameter]  public required UserDto User {get;set;}
    [Inject] public required AuthenticationStateProvider AuthenticationStateProvider {get; set;}
    [Inject] public required IUserService UserService {get; set;}
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        Voornaam = user.GetFirstname();
        Rol = user.GetRole();
        Naam = user.GetLastname();
        Email = user.GetEmail();

    }
}