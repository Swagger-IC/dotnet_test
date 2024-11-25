using System;
using System.Net.Http;
using System.Net.Http.Json;
using Rise.Shared.Users;

namespace Rise.Client.Users.Services;

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


    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var gebruiker = await httpClient.GetFromJsonAsync<UserDto>($"user/email/{email}");
        return gebruiker!;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var response = await httpClient.PostAsJsonAsync("user", createUserDto);

        response.EnsureSuccessStatusCode();

        var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();

        return createdUser!;
    }
}