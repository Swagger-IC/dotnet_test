using System.Threading;

namespace Rise.Shared.Users;

public interface IUserService
{
    // Task<IEnumerable<UserDto>> GetUsersAsync();
    // Task<UserDto> GetUserById(int userId);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> GetUserByEmailAsync(string email);

}
