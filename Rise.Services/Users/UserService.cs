using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rise.Persistence;
using Rise.Shared.Users;
using System.Linq;
using System.Linq.Expressions;

namespace Rise.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IManagementApiClient _managementApiClient;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext dbContext;

        public UserService(ApplicationDbContext dbContext, IManagementApiClient managementApiClient, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            _managementApiClient = managementApiClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
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

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _managementApiClient.Users.GetAsync(email);
            if (user == null) return null;
            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsBlocked = user.Blocked ?? false
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
        {
            var connectionName = _configuration["Auth0:Connection"];
            var existingDbUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == createDto.Email);

            if (existingDbUser != null)
            {
                throw new InvalidOperationException($"Er bestaat al een gebruiker met dit emailadres: {createDto.Email}.");
            }

            var dbUser = new Domain.Users.User //als Domain.Users hier niet staat komt er een error in verband met de auth0
            {
                Naam = createDto.LastName,
                Voornaam = createDto.FirstName,
                Email = createDto.Email,
            };

            dbContext.Users.Add(dbUser);
            await dbContext.SaveChangesAsync();

            var userCreateRequest = new UserCreateRequest
            {
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Password = createDto.Password, // Required for DB connection
                Connection = connectionName,
                UserMetadata = new
                {
                    UserId = dbUser.Id  // Hier geven we de lokale database ID mee
                }
            };

            var createdUser = await _managementApiClient.Users.CreateAsync(userCreateRequest);

            var assignRolesRequest = new AssignRolesRequest
            {
                Roles = new[] { createDto.RoleId }
            };

            await _managementApiClient.Users.AssignRolesAsync(createdUser.UserId, assignRolesRequest);

            return new UserDto
            {
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                IsBlocked = createdUser.Blocked ?? false
            };
        }
    }
}