using Microsoft.AspNetCore.Components;
using Rise.Shared.Users;

namespace Rise.Client.Users
{
    public partial class Index
    {
        [Inject] public required NavigationManager NavigationManager { get; set; }

        private IEnumerable<UserDto>? users;
        protected override async Task OnInitializedAsync()
        {
            users = await UserService.GetUsersAsync();
        }

        private void AddGebruiker()
        {
            NavigationManager.NavigateTo("/addGebruiker");
        }
    }
}
