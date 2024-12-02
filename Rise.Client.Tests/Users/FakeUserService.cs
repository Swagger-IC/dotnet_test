using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rise.Shared.Users;

namespace Rise.Client.Users;

    public class FakeUserService: IUserService
    {
    private readonly List<UserDto> _users;
    public FakeUserService()
    {

        _users = Enumerable.Range(1, 10)
                              .Select(i => new UserDto
                              {
                                Email = "michiel_murphy@outlook.com",
                                FirstName = "Michiel",
                                LastName = "Murphy",
                                IsBlocked = false
                              })
                              .ToList();
    }
    public Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return Task.FromResult(_users.AsEnumerable());
    }

    public Task<UserDto> GetUserByEmail(string email)  
    {
        var user = _users.FirstOrDefault(u => u.Email == email) ?? throw new InvalidOperationException($"User with email {email} not found");
        return Task.FromResult(user);
    }

    Task<UserDto> IUserService.CreateUserAsync(CreateUserDto createUserDto)
    {
        throw new NotImplementedException();
    }

    Task<UserDto?> IUserService.GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}