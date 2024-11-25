using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Rise.Client.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProductenlijstPlaywrightTests : PageTest
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
    public async Task ProductZoeken_Should_GefilterdeResultatenTonen_KaartView()
    {
        await Page.FillAsync("#product-search", "Chirurgische handschoenen");
        await Task.Delay(1000);

        // Wacht totdat de kaarten zijn geladen
        await Page.WaitForSelectorAsync("div.kaart");

        var productKaarten = await Page.QuerySelectorAllAsync("div.kaart");
        Console.WriteLine($"Aantal gevonden kaarten: {productKaarten}");

        var allMatch = true;

        foreach (var kaart in productKaarten)
        {
            // Zoek binnen elke kaart het element met de klasse 'naam'
            var titelElement = await kaart.QuerySelectorAsync("#naam");

            if (titelElement == null)
            {
                Console.WriteLine("Fout: Geen element met class 'naam' gevonden in een van de kaarten.");
                allMatch = false;
                break;
            }

            var productNaam = await titelElement.InnerTextAsync();
            Console.WriteLine($"Gevonden productnaam: {productNaam}");

            if (!productNaam.Contains("Chirurgische handschoenen", StringComparison.OrdinalIgnoreCase))
            {
                allMatch = false;
                break;
            }
        }

        Assert.True(allMatch, "Ze bevatten allemaal de geschreven zoekopdracht.");
    }

    [Test]
    public async Task ProductZoeken_Should_GefilterdeResultatenTonen_TableView()
    {
        await Page.ClickAsync("i");

        await Page.FillAsync("#product-search", "Chirurgische handschoenen");
        await Task.Delay(1000);

        await Page.WaitForSelectorAsync(".table tbody");
        var productRijen = await Page.QuerySelectorAllAsync(".table tbody tr");

        var allMatch = true;

        foreach (var rij in productRijen)
        {
            var naamCell = await rij.QuerySelectorAsync("td:nth-child(1)");
            if (naamCell != null)
            {
                var naamTekst = await naamCell.InnerTextAsync();
                if (!naamTekst.Contains("Chirurgische handschoenen", StringComparison.OrdinalIgnoreCase))
                {
                    allMatch = false;
                    break;
                }
            }
        }

        Assert.True(allMatch, "Ze bevatten allemaal de geschreven zoekopdracht.");
    }

    [Test]
    public async Task ToggleView_Should_NaarTableViewWisselen()
    {
        await Page.ClickAsync("i");

        bool isTabelView = await Page.IsVisibleAsync("table");
        Assert.IsTrue(isTabelView, "De tabelweergave zou zichtbaar moeten zijn.");
    }

    [Test]
    public async Task ToggleView_Should_NaarTableViewWisselen_DanTerugKaartView()
    {
        await Page.ClickAsync("i");

        bool isTabelView = await Page.IsVisibleAsync("table");
        Assert.IsTrue(isTabelView, "De tabelweergave zou zichtbaar moeten zijn.");

        await Page.ClickAsync("i");

        var productKaart = Page.Locator(".productkaart").Filter(new() { HasText = "Chirurgische handschoenen" }).First;


        Assert.IsNotNull(productKaart, "Productkaart zou moeten zijn geselecteerd.");
    }

    [Test]
    public async Task ProductVerwijderen_Should_ProductVerwijderdWorden()
    {
        // Vul het zoekveld in om een specifiek product te zoeken
        await Page.FillAsync("#product-search", "Slimme Gast");
        await Task.Delay(1000);

        // Wacht totdat de productkaarten geladen zijn
        await Page.WaitForSelectorAsync("div.kaart");

        // Zoek de kaart van het product
        var productKaart = await Page.QuerySelectorAsync("div.kaart:has-text('Slimme Gast')");

        // Controleer of het product zichtbaar is voordat we het verwijderen
        Assert.IsNotNull(productKaart, "Het product zou zichtbaar moeten zijn voordat we het verwijderen.");

        // Klik op de verwijderknop binnen de productkaart
        var deleteButton = await productKaart.QuerySelectorAsync(".btn-danger");
        if (deleteButton != null)
        {
            // Stel een dialooglistener in voor het confirm dialoog
            Page.Dialog += async (sender, e) =>
            {
                if (e.Type == DialogType.Confirm)
                {
                    // Bevestig de actie door op "OK" te klikken
                    await e.AcceptAsync();  // Gebruik AcceptAsync() in plaats van Accept()
                }
            };

            // Klik op de verwijderknop
            await deleteButton.ClickAsync();

            // Wacht tot het product verdwijnt uit de lijst
            await Page.WaitForSelectorAsync("div.kaart", new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden });

            // Controleer of de productkaart nu niet meer zichtbaar is
            var deletedProductKaart = await Page.QuerySelectorAsync("div.kaart:has-text('Chirurgische handschoenen')");
            Assert.IsNull(deletedProductKaart, "Het product zou verwijderd moeten zijn."); ;
        }
        else
        {
            Assert.Fail("De verwijderknop was niet gevonden voor het product.");
        }
    }
}