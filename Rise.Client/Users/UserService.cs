using System;
using System.Net.Http.Json;
using Rise.Shared.Users;

namespace Rise.Client.Users;

public class UserService(HttpClient httpClient) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await httpClient.GetFromJsonAsync<UserDto[]>("user");
        return users!;
    }

    public async Task<UserDto> GetUserById(int id)
    {
        
        var gebruiker = await httpClient.GetFromJsonAsync<UserDto>($"user/{id}");
        return gebruiker!; 
    }

    
    public async Task<UserDto> GetUserByEmail(string email)
    {
        var gebruiker = await httpClient.GetFromJsonAsync<UserDto>($"user/email/{email}");
        return gebruiker!;
    }
}