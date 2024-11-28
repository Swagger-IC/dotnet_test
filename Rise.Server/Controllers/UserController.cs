using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Administrator")]
public class UserController : ControllerBase
{
    // private readonly IUserService userService;

    // public UserController(IUserService userService)
    // {
    //     this.userService = userService;
    // }

    // [HttpGet]
    // public async Task<IEnumerable<UserDto>> Get()
    // {
    //     var users = await userService.GetUsersAsync();
    //     return users;
    // }

    // [HttpGet("{id}")]
    // public async Task<ActionResult<UserDto>> GetUserById(int id)
    // {
    //     var user = await userService.GetUserById(id);
    //     if (user == null)
    //         return NotFound();
    //     return user;
    // }

    // [HttpGet("email/{email}")]
    // public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    // {
    //     var user = await userService.GetUserByEmail(email);
    //     if (user == null)
    //         return NotFound();
    //     return user;
    // }

    private readonly IManagementApiClient _managementApiClient;

    public UserController(IManagementApiClient managementApiClient)
    {
        _managementApiClient = managementApiClient;
    }

    [HttpGet]
    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
        return users.Select(x => new UserDto
        {
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            IsBlocked = x.Blocked ?? false,
        });
    }
}