using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Auth0.Core.Exceptions;
using Rise.Shared.Roles;

namespace Rise.Server.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class RolesController : ControllerBase
{

    private readonly IManagementApiClient _managementApiClient;
    private readonly IConfiguration _configuration;

    public RolesController(IManagementApiClient managementApiClient, IConfiguration configuration) //configuration is om op te halen uit appsettings
    {
        _managementApiClient = managementApiClient;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IEnumerable<RolDto>> GetRoles()
    {
        var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest(), new PaginationInfo());
        return roles.Select(x => new RolDto
        {
            Id = x.Id,                
            Name = x.Name,            
            Description = x.Description 
        });
    }
}