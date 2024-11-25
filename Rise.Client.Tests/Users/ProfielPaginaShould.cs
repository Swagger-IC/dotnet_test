using Microsoft.AspNetCore.Components.Authorization;
using Rise.Client.Users.Components;
using Rise.Shared.Users;
using Shouldly;
using Moq;
using System.Security.Claims;
using Rise.Client.Products.Components;

namespace Rise.Client.Users
{
    public class ProfielPaginaShould : TestContext
    {
        public ProfielPaginaShould()
        {
            Services.AddScoped<IUserService, FakeUserService>();
            Services.AddAuthorizationCore();
        }

        [Fact]
        public void ShowHeaderInfo()
        {
            var authState = new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("given_name", "Jos"),
                    new Claim("family_name", "Achternaam"),
                    new Claim(ClaimTypes.Role, "Administrator"),
                }, "mock")));

            var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
            mockAuthStateProvider
                .Setup(provider => provider.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            Services.AddSingleton<AuthenticationStateProvider>(mockAuthStateProvider.Object);


            var cut = RenderComponent<Profiel>();

            var nameElement = cut.Find("h1");
            nameElement.InnerHtml.ShouldContain("Welkom, Jos");

            var roleElement = cut.Find("p");
            roleElement.InnerHtml.ShouldContain("Administrator");
        }

        [Fact]
        public void ShowPersoonlijkInfoKaart()
        {
            // Arrange: Maak een mock voor de authenticatiestatus
            var authState = new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
            new Claim("given_name", "Jos"),
            new Claim("family_name", "Achternaam"),
            new Claim(ClaimTypes.Role, "Administrator"),
                }, "mock")));

            var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
            mockAuthStateProvider
                .Setup(provider => provider.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            // Voeg de mock AuthenticationStateProvider toe aan de services
            Services.AddSingleton<AuthenticationStateProvider>(mockAuthStateProvider.Object);

            // Arrange: Render de Profielpagina met de benodigde parameters
            var cut = RenderComponent<Profiel>();

            // Act: Zoek naar de elementen in de header en controleer of de naam en rol correct zijn
            var nameElement = cut.Find("h1");
            nameElement.InnerHtml.ShouldContain("Welkom, Jos");

            var roleElement = cut.Find("p");
            roleElement.InnerHtml.ShouldContain("Administrator");

            // Act: Zoek naar de <b> elementen voor "Voornaam", "Naam" en "Email"
            var voornaamElement = cut.Find("p:nth-of-type(1) b");
            voornaamElement.InnerHtml.ShouldContain("Voornaam:");

            var naamElement = cut.Find("p:nth-of-type(2) b");
            naamElement.InnerHtml.ShouldContain("Naam:");

            var emailElement = cut.Find("p:nth-of-type(3) b");
            emailElement.InnerHtml.ShouldContain("Email:");

            
        }

        [Fact]
        public void ShowProductToevoegenKaartForAdmin()
        {
            // Arrange: Maak een mock voor de authenticatiestatus
            var authState = new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                new Claim("given_name", "Jos"),
                new Claim("family_name", "Achternaam"),
                new Claim(ClaimTypes.Role, "Administrator"),
                }, "mock")));

            var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
            mockAuthStateProvider
                .Setup(provider => provider.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            // Voeg de mock AuthenticationStateProvider toe aan de services
            Services.AddSingleton<AuthenticationStateProvider>(mockAuthStateProvider.Object);

            // Arrange: Render de Profielpagina met de benodigde parameters
            var cut = RenderComponent<Profiel>(parameters => parameters
                .Add(p => p.Rol, "Administrator")
            );

            // Act: Zoek naar de ProductToevoegenKaart component
            var productToevoegenComponent = cut.FindComponent<ProductToevoegenKaart>();

            // Assert: Controleer of de titel en de knop correct worden weergegeven
            var titleElement = productToevoegenComponent.Find("b");
            titleElement.MarkupMatches("<b style=\"font-size: 20px;\">Product toevoegen</b>");

            var buttonElement = productToevoegenComponent.Find("button");
            buttonElement.MarkupMatches("<button class=\"btn btn-dark\" style=\"width: auto;\">Voeg product toe</button>");
        }

        [Fact]
        public void NotShowProductToevoegenKaartForNonAdmin()
        {
            // Arrange
            var authState = new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                        new System.Security.Claims.Claim("role", "Gebruiker") // Niet-admin rol
                }, "mock")
            ));

            var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
            mockAuthStateProvider
                .Setup(provider => provider.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            Services.AddSingleton<AuthenticationStateProvider>(mockAuthStateProvider.Object);

            var cut = RenderComponent<Profiel>(parameters => parameters
                .Add(p => p.Rol, "Gebruiker")  // Zorg ervoor dat je ook andere parameters toevoegt
            );

            // Act & Assert
            Assert.Throws<Bunit.Rendering.ComponentNotFoundException>(() => cut.FindComponent<ProductToevoegenKaart>());
        }

    }
}
