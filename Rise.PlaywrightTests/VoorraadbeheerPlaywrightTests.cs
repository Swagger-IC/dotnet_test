using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Rise.Persistence;

namespace Rise.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class VoorraadbeheerPlaywrightTests : PageTest
{

    private ApplicationDbContext? _dbContext;
    
    private const string Email = "michiel_murphy@outlook.com";
    private const string Password = "Superkat55!";

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Hogent.RiseDb;Trusted_Connection=True;") // Use the actual connection string for your test DB
            .Options;

        _dbContext = new ApplicationDbContext(options);

        await Page.GotoAsync("https://localhost:5001");
        await Page.FillAsync("label:has-text('Email address') + input", Email);
        await Page.FillAsync("label:has-text('Password') + input", Password);
        await Page.ClickAsync("button:has-text('Continue')");
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_dbContext != null)
        {
            var product = await _dbContext.Products.FirstAsync(p => p.Name == "Pleister");
            product.Quantity = 50; // Reset naar originele state (hoe het in de seeding file staat)
            await _dbContext.SaveChangesAsync();

            await _dbContext.DisposeAsync();
        }
    }

    [Test]
    public async Task Voorraadbeheer_Should_TweeProductenTonen()
    {
        await Page.GotoAsync("https://localhost:5001/voorraadbeheer");
        await Expect(Page.Locator("h5:text('Voorraadbeheer')")).ToBeVisibleAsync();
        await Expect(Page.Locator("table")).ToBeVisibleAsync(); 

        await Expect(Page.Locator("table")).ToBeVisibleAsync();

        var rows = Page.Locator("table tbody tr");

        var rowCount = await rows.CountAsync();
        Assert.That(rowCount, Is.EqualTo(2), "Tabel heeft 2 rijen");


        var firstProductName = await rows.Nth(0).Locator("td:nth-child(1)").TextContentAsync();
        var secondProductName = await rows.Nth(1).Locator("td:nth-child(1)").TextContentAsync();

        Assert.That(firstProductName?.Trim(), Is.EqualTo("Pleister"));
        Assert.That(secondProductName?.Trim(), Is.EqualTo("Vernevelaar"));
    }

    [Test]
    public async Task Voorraadbeheer_Should_Add_One_To_First_Product_And_Update_Quantity()
    {
        await Page.GotoAsync("https://localhost:5001/voorraadbeheer");
        await Task.Delay(1000);
        await Expect(Page.Locator("h5:text('Voorraadbeheer')")).ToBeVisibleAsync();
        await Expect(Page.Locator("table")).ToBeVisibleAsync();

        var rows = Page.Locator("table tbody tr");

        var rowCount = await rows.CountAsync();
        Assert.That(rowCount, Is.EqualTo(2), "Tabel heeft 2 rijen");

        var firstProductQuantityLocator = rows.Nth(0).Locator("td:nth-child(2)");
        var initialQuantity = await firstProductQuantityLocator.TextContentAsync();

        if (_dbContext != null)
        {
            var product = await _dbContext.Products.FirstAsync(p => p.Name == "Pleister");
            product.Quantity = 50; // Reset naar originele state (hoe het in de seeding file staat)
            await _dbContext.SaveChangesAsync();
        }

        Assert.That(initialQuantity?.Trim(), Is.EqualTo("50"), "De hoeveelheid van het eerste product zou 50 moeten zijn.");

        var quantityInputLocator = rows.Nth(0).Locator("td:nth-child(5) input");

        await quantityInputLocator.FillAsync("1"); 
        await Page.ClickAsync("button:has-text('Bestel')"); 

        await Page.WaitForTimeoutAsync(1000);

        var updatedQuantity = await firstProductQuantityLocator.TextContentAsync();

        var updatedQuantityInt = int.Parse(updatedQuantity?.Trim() ?? "0");
        Assert.That(updatedQuantityInt, Is.EqualTo(51), "De hoeveelheid van het eerste product zou met 1 moeten zijn verhoogd.");
    }

    
}
