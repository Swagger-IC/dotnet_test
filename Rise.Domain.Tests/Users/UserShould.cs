using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Users;

/// <summary>
/// Domain Tests for User using xUnit and Shouldly
/// </summary>
public class UserShould
{
    [Fact]
    public void BeCreated()
    {
        User user = new()
        {
            Voornaam = "John",
            Naam = "Doe",
            Email = "john.doe@example.com",
            
        };

        user.Voornaam.ShouldBe("John");
        user.Naam.ShouldBe("Doe");
        user.Email.ShouldBe("john.doe@example.com");
        
    }

    [Theory]
    [InlineData(null, "Doe", "john.doe@example.com")]
    [InlineData("   ", "Doe", "john.doe@example.com")]
    [InlineData("John", null, "john.doe@example.com")]
    [InlineData("John", "   ", "john.doe@example.com")]
    [InlineData("John", "Doe", null)]
    [InlineData("John", "Doe", "   ")]
    public void NotBeCreatedWithInvalidValues(string? voornaam, string? naam, string? email)
    {
        Action act = () =>
        {
            User user = new()
            {
                Voornaam = voornaam!,
                Naam = naam!,
                Email = email!,
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null, "Doe", "john.doe@example.com")]
    [InlineData("   ", "Doe", "john.doe@example.com")]
    [InlineData("John", null, "john.doe@example.com")]
    [InlineData("John", "   ", "john.doe@example.com")]
    [InlineData("John", "Doe", null)]
    [InlineData("John", "Doe", "   ")]
    public void NotAllowPropertyChangeToInvalidValues(string? voornaam, string? naam, string? email)
    {
        User user = new()
        {
            Voornaam = "John",
            Naam = "Doe",
            Email = "john.doe@example.com",
            
        };

        Action act = () =>
        {
            user.Voornaam = voornaam!;
            user.Naam = naam!;
            user.Email = email!;
        };

        act.ShouldThrow<ArgumentException>();
    }
}
