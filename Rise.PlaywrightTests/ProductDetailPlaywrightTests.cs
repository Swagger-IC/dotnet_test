using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Rise.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProductDetailPlaywrightTests : PageTest
{
    private const string Email = "michiel_murphy@outlook.com";
    private const string Password = "Superkat55!";

    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync($"https://localhost:5001");
        await Page.FillAsync("label:has-text('Email address') + input", Email);
        await Page.FillAsync("label:has-text('Password') + input", Password);
        await Page.ClickAsync("button:has-text('Continue')");
    }

    [Test]
    public async Task ProductKlikken_KaartView_Should_ProductDetailsTonen()
    {
        var productCardLocator = Page.Locator("#app div").Filter(new() { HasText = "Chirurgische handschoenen" }).Nth(3);
        var productId = await productCardLocator.GetAttributeAsync("data-id");
        await productCardLocator.ClickAsync();
        string expectedUrl = $"https://localhost:5001/product/{productId}";
        await Expect(Page).ToHaveURLAsync(expectedUrl);

        string expectedProductNaam = "Chirurgische handschoenen";
        var productDetailTitleLocator = Page.Locator("role=heading[name='Chirurgische handschoenen']");
        var titleText = await productDetailTitleLocator.InnerTextAsync();
        Assert.That(titleText, Is.EqualTo(expectedProductNaam));
        Assert.Pass("De URL en productnaam zijn correct.");
    }

    [Test]
    public async Task ProductKlikken_TableView_Should_ProductDetailsTonen()
    {
        await Page.ClickAsync("i");
        string productId = "10";
        var productRowLocator = Page.Locator($"tr.rij[data-id='{productId}']");
        await productRowLocator.ClickAsync();
        string expectedUrl = $"https://localhost:5001/product/{productId}";
        await Expect(Page).ToHaveURLAsync(expectedUrl);

        string expectedProductNaam = "Glucometer";
        var productDetailTitleLocator = Page.Locator("role=heading[name='Glucometer']");
        var titleText = await productDetailTitleLocator.InnerTextAsync();
        Assert.That(titleText, Is.EqualTo(expectedProductNaam));

        Assert.Pass("De URL en productnaam zijn correct.");
    }

    [Test]
    public async Task ProductToevoegen_Should_ToonOverzicht()
    {
        await VoegProductToe("Hechtingen");

        // overzicht
        var articleLocator = Page.Locator("role=article");
        var divLocator = articleLocator.Locator("div").Filter(new() { HasText = "OverzichtHechtingen" }).Nth(3);
        var paragraphLocator = divLocator.Locator("p").Filter(new() { HasTextRegex = new Regex("^Hechtingen$") });
        var paragraphName = await paragraphLocator.InnerTextAsync();
        string expectedNaam = "Hechtingen";
        Assert.That(paragraphName, Is.EqualTo(expectedNaam), "The paragraph name does not match the expected value.");

        var spinButtonLocator = articleLocator.Locator("role=spinbutton").Nth(1);
        var value = await spinButtonLocator.InputValueAsync();
        Assert.That(value, Is.EqualTo("5"));

        Assert.Pass("Het product is correct toegevoegd aan het overzicht.");
    }

    // twee toevoegen, eentje verwijderen
    [Test]
    public async Task TweeProductenToevoegen_EenProductVerwijderen_Should_ToonOverzichtVanEenProduct()
    {
        await VoegProductToe("Hechtingen");

        // terug naar productenlijst
        var buttonLocator = Page.Locator("role=button[name='Naar overzicht']");
        await buttonLocator.ClickAsync();

        // naar tweede pagina
        buttonLocator = Page.Locator("role=button[name='2']");
        await buttonLocator.ClickAsync();

        await VoegProductToe("Spuit Lokaal: Lokaal");

        await ControleerOverzicht(new[] { "Hechtingen", "Spuit" }, new[] { "5", "5" });

        // eerste lijn verwijderen
        var articleLocator = Page.Locator("role=article");
        var lijn1 = articleLocator.Locator("div:nth-child(2) > .card > .card-body > div").First;
        buttonLocator = lijn1.Locator("#trash");
        await buttonLocator.ClickAsync();

        var divLocator = articleLocator.Locator("div").Filter(new() { HasText = "OverzichtSpuit" }).Nth(3);
        var paragraphs = divLocator.Locator("p");

        var paragraphLocator = divLocator.Locator("p").Filter(new() { HasTextRegex = new Regex("^Spuit$") });
        var paragraphName = await paragraphLocator.InnerTextAsync();
        string expectedNaam = "Spuit";
        Assert.Multiple(() =>
        {
            Assert.That(paragraphs.CountAsync().Result, Is.EqualTo(1));
            Assert.That(paragraphName, Is.EqualTo(expectedNaam), "The paragraph name does not match the expected value.");
        });
    }

    private async Task VoegProductToe(string text)
    {
        var productCardLocator = Page.Locator("#app div").Filter(new() { HasText = text }).Nth(3);
        await productCardLocator.ClickAsync();
        var spinButtonLocator = Page.GetByRole(AriaRole.Spinbutton).Nth(0);
        for (int i = 0; i < 4; i++)
        {
            await spinButtonLocator.PressAsync("ArrowUp");
        }
        var value = await spinButtonLocator.InputValueAsync();
        Assert.That(value, Is.EqualTo("5"));
        var buttonLocator = Page.Locator("role=button[name='Voeg toe aan overzicht']");
        await buttonLocator.ClickAsync();
    }

    private async Task ControleerOverzicht(string[] expectedProductNames, string[] expectedValues)
    {
        var articleLocator = Page.Locator("role=article");
        var divLocator = articleLocator.Locator("div")
            .Filter(new() { HasText = "OverzichtHechtingen Spuit" }).Nth(3);
        var paragraphs = divLocator.Locator("p");
        var paragraphTexts = await paragraphs.AllTextContentsAsync();

        Assert.Multiple(() =>
        {
            for (int i = 0; i < expectedProductNames.Length; i++)
            {
                Assert.That(paragraphTexts[i], Is.EqualTo(expectedProductNames[i]),
                    $"The paragraph name {i + 1} does not match the expected value.");
            }
        });

        for (int i = 0; i < expectedValues.Length; i++)
        {
            var spinButtonLocator = articleLocator.Locator("role=spinbutton").Nth(i + 1);
            var value = await spinButtonLocator.InputValueAsync();
            Assert.That(value, Is.EqualTo(expectedValues[i]), $"Spin button value {i + 1} does not match expected.");
        }
    }
}