
using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Rise.Client.Pages
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ProductToevoegenPlaywrightTests : PageTest
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
        public async Task VoegProductToe_VultFormulierIn_Should_ControleertValidaties_EnBevestigtToevoeging()
        {
            // Navigeer naar producttoevoegen pagina
            await Page.ClickAsync("#toevoegen");

            // Vul het productformulier in
            await Page.FillAsync("#name", "Nieuw Product");
            await Page.FillAsync("#location", "Locatie A");
            await Page.FillAsync("#description", "Beschrijving van het product.");
            await Page.FillAsync("#barcode", "1234567890123");
            await Page.FillAsync("#quantity", "10");
            await Page.FillAsync("#minStock", "5");
            await Page.FillAsync("#keywords", "voorbeeld, product, test");
            await Page.SelectOptionAsync("#reusable", "false");
            await Page.SelectOptionAsync("#leverancier", "1");

            await Page.SetInputFilesAsync("#imageUpload", "C:/HoGent24-25/RISE/images/bloeddrukmeter.webp");


            await Page.ClickAsync("button[type='submit']");

            bool hasErrors = await Page.Locator(".validation-message").CountAsync() > 0;
            Assert.False(hasErrors, "Er zijn validatiefouten in het formulier.");

            
            await Task.Delay(1000);
            await Page.FillAsync("#product-search", "Nieuw Product");
            await Task.Delay(1000);

            await Page.WaitForSelectorAsync("div.kaart");

            var productKaarten = await Page.QuerySelectorAllAsync("div.kaart");
            Console.WriteLine($"Aantal gevonden kaarten: {productKaarten}");

            var allMatch = true;

            foreach (var kaart in productKaarten)
            {
                var titelElement = await kaart.QuerySelectorAsync("#naam");

                if (titelElement == null)
                {
                    Console.WriteLine("Fout: Geen element met class 'naam' gevonden in een van de kaarten.");
                    allMatch = false;
                    break;
                }

                var productNaam = await titelElement.InnerTextAsync();
                Console.WriteLine($"Gevonden productnaam: {productNaam}");

                if (!productNaam.Contains("Nieuw Product", StringComparison.OrdinalIgnoreCase))
                {
                    allMatch = false;
                    break;
                }
            }

            Assert.True(allMatch, "Ze bevatten allemaal de geschreven zoekopdracht.");
            
        }

        [Test]
        public async Task ValidatieControleren_BijIncompleetFormulier()
        {
            await Page.ClickAsync("#toevoegen");

            await Page.ClickAsync("button[type='submit']");

            await Page.WaitForSelectorAsync(".validation-message");
            bool hasErrors = await Page.Locator(".validation-message").CountAsync() > 0;

            Assert.True(hasErrors, "Validatiefouten zouden moeten verschijnen.");
        }


    }
}
