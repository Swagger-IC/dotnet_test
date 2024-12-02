using System.Net.Http.Json;
using Rise.Shared.Roles;

namespace Rise.Client.Users.Services;

public class RolService(HttpClient httpClient) : IRolService
{
    public async Task<IEnumerable<RolDto>> GetRolesAsync()
    {
        var roles = await httpClient.GetFromJsonAsync<IEnumerable<RolDto>>("roles");
        return roles!;
    }
}