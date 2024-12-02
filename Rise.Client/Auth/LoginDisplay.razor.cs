using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Rise.Client.Auth;

public partial class LoginDisplay
{    
    [Inject] public required NavigationManager Navigation { get; set; }
    public void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout");
    }
    
}
