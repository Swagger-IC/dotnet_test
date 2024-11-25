using Rise.Shared.Products;
using Xunit.Abstractions;
using Shouldly;
using Rise.Client.Products.Services;
using System.Threading.Tasks;

namespace Rise.Client.Products;

public class ProductenlijstShould : TestContext
{
    public ProductenlijstShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
        Services.AddScoped<IProductService, FakeProductService>();
        Services.AddScoped<ProductenlijstStatus>();
    }

    private IRenderedComponent<Productenlijst> RenderProductenlijst()
    {
        return RenderComponent<Productenlijst>();
    }

    [Fact]
    public void ShowsProductsInitially()
    {
        var cut = RenderProductenlijst();

        // Aantal producten wordt gecontroleerd
        cut.FindAll(".row .card-title").Count.ShouldBe(7);
    }

    [Fact]
    public void ShowsReusableProducts()
    {
        var cut = RenderProductenlijst();

        // Klik op de 'Uitlenen' knop om herbruikbare producten te tonen
        cut.Find("button:contains('Uitlenen')").Click();

        // Controleer of er 3 herbruikbare producten worden weergegeven
        cut.FindAll(".row .card-title").Count.ShouldBe(3);
    }


    [Fact]
    public void SearchOnDescriptionInitialScreen()
    {
        var cut = RenderProductenlijst();

        // Voer een algemene zoekterm in
        var inputElement = cut.Find("#product-search");
        inputElement.Change("Product");

        // Controleer of alle 7 producten worden weergegeven
        var productCards = cut.FindAll(".row .card-title");
        productCards.Count.ShouldBe(7);
    }

    // Test om herbruikbare producten te tonen door de knop 'Uitlenen' te gebruiken
    [Fact]
    public void FilterProductsWhenUitlenenButtonIsClicked()
    {
        var cut = RenderProductenlijst();

        // Klik op de knop 'Uitlenen'
        cut.Find("button:contains('Uitlenen')").Click();

        // Controleer of de juiste producten worden getoond
        var productCards = cut.FindAll(".row .card-title");
        productCards.Count.ShouldBe(3);
    }
}