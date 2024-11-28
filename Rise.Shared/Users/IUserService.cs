using Rise.Shared.Products;
using System.Threading;

namespace Rise.Shared.Users;

public interface IUserService
{
    // Task<IEnumerable<UserDto>> GetUsersAsync();
    // Task<UserDto> GetUserById(int userId);
    // Task<UserDto> GetUserByEmail(string email);
    Task<IEnumerable<UserDto>> GetUsersAsync();

}
