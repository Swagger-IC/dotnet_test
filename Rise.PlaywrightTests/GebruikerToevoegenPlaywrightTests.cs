
using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Rise.Client.Pages
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class GebruikerToevoegenPlaywrightTests : PageTest
    {
        private const string Email = "michiel_murphy@outlook.com";
        private const string Password = "Superkat55!";

        [SetUp]
        public async Task SetUp()
        {
            await Page.GotoAsync("https://localhost:5001/users");
            await Page.FillAsync("label:has-text('Email address') + input", Email);
            await Page.FillAsync("label:has-text('Password') + input", Password);
            await Page.ClickAsync("button:has-text('Continue')");
        }

        [Test]
        public async Task VoegGebruikerToe_VultFormulierIn_ControleertValidaties_EnBevestigtToevoeging()
        {
            // Klik op de knop om een gebruiker toe te voegen
            await Page.ClickAsync("#toevoegenUser");

            // Vul het formulier in
            await Page.FillAsync("#firstname", "Jan");
            await Page.FillAsync("#name", "Jansen");
            await Page.FillAsync("#email", "jan.jansen@example.com");
            await Page.FillAsync("#password", "Supergeheim123!");
            await Page.SelectOptionAsync("#rol", "Docent"); // Selecteer een geldige rol-id

            // Klik op de submit-knop
            await Page.ClickAsync("button[type='submit']");

            // Controleer of er geen validatiefouten zijn
            bool hasErrors = await Page.Locator(".validation-message").CountAsync() > 0;
            Assert.False(hasErrors, "Er zijn validatiefouten in het formulier.");



            // Controleer of de gebruiker in de lijst verschijnt
            //reload en een van de wait forselectorasyncs mogen weg als de problemen met state opgelost zijn
            await Page.WaitForSelectorAsync("table");
            await Page.ReloadAsync();
            await Page.WaitForSelectorAsync("table");
            var gebruikers = await Page.QuerySelectorAllAsync("tbody tr");
            Assert.That(gebruikers, Is.Not.Empty, "Geen gebruikers gevonden in de tabel.");

            var gevonden = false;
            foreach (var gebruiker in gebruikers)
            {
                var emailElement = await gebruiker.QuerySelectorAsync("th");
                var emailText = await emailElement!.InnerTextAsync();
                if (emailText == "jan.jansen@example.com")
                {
                    gevonden = true;
                    break;
                }
            }
            Assert.True(gevonden, "De toegevoegde gebruiker is niet gevonden in de lijst.");
        }


        [Test]
        public async Task ValidatieControleren_BijIncompleetFormulier()
        {
            // Klik op de knop om een gebruiker toe te voegen
            await Page.ClickAsync("#toevoegenUser");

            // Klik op de submit-knop zonder iets in te vullen
            await Page.ClickAsync("button[type='submit']");

            // Controleer of er validatiefouten verschijnen
            await Page.WaitForSelectorAsync(".validation-message");
            bool hasErrors = await Page.Locator(".validation-message").CountAsync() > 0;

            Assert.True(hasErrors, "Validatiefouten zouden moeten verschijnen.");
        }



    }
}