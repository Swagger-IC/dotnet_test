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
            TelNr = "0123456789",
            Rol = Rol.STUDENT
        };

        user.Voornaam.ShouldBe("John");
        user.Naam.ShouldBe("Doe");
        user.Email.ShouldBe("john.doe@example.com");
        user.TelNr.ShouldBe("0123456789");
        user.Rol.ShouldBe(Rol.STUDENT);
    }

    [Theory]
    [InlineData(null, "Doe", "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("   ", "Doe", "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("John", null, "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("John", "   ", "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("John", "Doe", null, "0123456789", Rol.STUDENT)]
    [InlineData("John", "Doe", "   ", "0123456789", Rol.STUDENT)]
    [InlineData("John", "Doe", "john.doe@example.com", null, Rol.STUDENT)]
    [InlineData("John", "Doe", "john.doe@example.com", "   ", Rol.STUDENT)]
    public void NotBeCreatedWithInvalidValues(string? voornaam, string? naam, string? email, string? telNr, Rol rol)
    {
        Action act = () =>
        {
            User user = new()
            {
                Voornaam = voornaam!,
                Naam = naam!,
                Email = email!,
                TelNr = telNr!,
                Rol = rol
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null, "Doe", "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("   ", "Doe", "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("John", null, "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("John", "   ", "john.doe@example.com", "0123456789", Rol.STUDENT)]
    [InlineData("John", "Doe", null, "0123456789", Rol.STUDENT)]
    [InlineData("John", "Doe", "   ", "0123456789", Rol.STUDENT)]
    [InlineData("John", "Doe", "john.doe@example.com", null, Rol.STUDENT)]
    [InlineData("John", "Doe", "john.doe@example.com", "   ", Rol.STUDENT)]
    public void NotAllowPropertyChangeToInvalidValues(string? voornaam, string? naam, string? email, string? telNr, Rol rol)
    {
        User user = new()
        {
            Voornaam = "John",
            Naam = "Doe",
            Email = "john.doe@example.com",
            TelNr = "0123456789",
            Rol = Rol.STUDENT
        };

        Action act = () =>
        {
            user.Voornaam = voornaam!;
            user.Naam = naam!;
            user.Email = email!;
            user.TelNr = telNr!;
            user.Rol = rol;
        };

        act.ShouldThrow<ArgumentException>();
    }
}
