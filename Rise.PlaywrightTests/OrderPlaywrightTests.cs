using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Rise.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class OrderPlaywrightTests : PageTest
{
    private const string Email = "michiel_murphy@outlook.com";
    private const string Password = "Superkat55!";

    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync("https://localhost:5001");
        await Page.FillAsync("label:has-text('Email address') + input", Email);
        await Page.FillAsync("label:has-text('Password') + input", Password);
        await Page.ClickAsync("button:has-text('Continue')");
    }

    [Test]
    public async Task Winkelmand_BestellingPlaatsen_Should_ToonSuccesBericht()
    {
        // Voeg een product toe aan het winkelmandje
        await VoegProductToeAanWinkelmand("chirurgische Handschoenen");

        var winkelkarButton = Page.Locator(".cart-container button.nav-link");
        await winkelkarButton.ClickAsync();

        // Controleer of de navigatie naar de winkelmandpagina correct is
        await Expect(Page).ToHaveURLAsync("https://localhost:5001/winkelmand");

        // Controleer of het winkelmandje niet leeg is
        var winkelmandLijst = Page.Locator(".list-group");
        Assert.That(await winkelmandLijst.CountAsync(), Is.GreaterThan(0), "Winkelmand is leeg, terwijl er producten toegevoegd hadden moeten zijn.");

        // Klik op de knop "Bestelling plaatsen"
        var plaatsBestellingButton = Page.Locator("button:has-text('Bestelling plaatsen')");
        await plaatsBestellingButton.ClickAsync();

        // Controleer of het succesbericht verschijnt
        var alert = Page.Locator(".alert-success");
        await Expect(alert).ToBeVisibleAsync();
        var alertText = await alert.InnerTextAsync();
        Assert.That(alertText, Is.EqualTo("Bestelling succesvol geplaatst!"), "Succesbericht komt niet overeen.");
    }

    private async Task VoegProductToeAanWinkelmand(string productNaam)
    {
        // Zoek het product in de lijst en klik om toe te voegen
        var productCardLocator = Page.Locator("#app div").Filter(new() { HasText = productNaam }).Nth(3);
        await productCardLocator.ClickAsync();

        // Pas de hoeveelheid aan (optioneel)
        var spinButtonLocator = Page.Locator("role=spinbutton").Nth(0);
        for (int i = 0; i < 4; i++)
        {
            await spinButtonLocator.PressAsync("ArrowUp");
        }

        // Voeg het product toe aan het winkelmandje
        var voegToeButton = productCardLocator.Locator("button:has-text('Voeg toe aan overzicht')");
        await voegToeButton.ClickAsync();
    }
}

