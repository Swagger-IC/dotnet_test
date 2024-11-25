using Xunit;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Rise.Shared.Users; 
using Xunit.Abstractions;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace Rise.Server.Tests.Controllers
{
    

    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly ITestOutputHelper _output;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _httpClient = factory.CreateClient();
            _output = output;
        }

        [Fact]
        [Authorize(Roles = "Administrator")]
        public async Task Create_CreatesNewUser()
        {
            // Arrange: Create a new User object
            var newUser = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "SecurePassword123!",
                RoleId = "rol_4ofpSMHqJNRw40YZ"
            };

            // Act: Send a POST request to create the user
            var response = await _httpClient.PostAsJsonAsync("user", newUser);

            // Assert: Check if the response indicates success
            response.EnsureSuccessStatusCode(); // Expecting 201 Created

            var responseBody = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseBody);

            // Optionally, parse the response if the API returns the created user
            var createdUser = JsonSerializer.Deserialize<UserDto>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(createdUser);
            Assert.Equal(newUser.Email, createdUser.Email);

        }

    }

}
