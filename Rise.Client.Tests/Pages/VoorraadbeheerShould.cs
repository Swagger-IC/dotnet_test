using Rise.Shared.Products;
using Xunit.Abstractions;
using Shouldly;

namespace Rise.Client.Products
{
    public class VoorraadbeheerShould : TestContext
    {
        public VoorraadbeheerShould(ITestOutputHelper outputHelper)
        {
            Services.AddXunitLogger(outputHelper);
            Services.AddScoped<IProductService, FakeProductService>(); // Ensure you have a mock implementation of IProductService
        }

        [Fact]
        public void ShowProductsAfterLoading()
        {
            var cut = RenderComponent<Voorraadbeheer>();

            cut.WaitForState(() => cut.FindAll(".table tbody tr").Count > 0);

            var productRows = cut.FindAll(".table tbody tr");
            productRows.Count.ShouldBe(2); 
        }

        [Fact]
        public void SortProductsByQuantity()
        {
            var cut = RenderComponent<Voorraadbeheer>();

            cut.WaitForState(() => cut.FindAll(".table tbody tr").Count > 0);

            var initialProductName = cut.Find(".table tbody tr:first-child td:first-child").InnerHtml;

            cut.Find("#quantity-header").Click();

            cut.WaitForState(() => cut.FindAll(".table tbody tr").Count > 0);

            var secondProductNameAfterSorting = cut.Find(".table tbody tr:nth-child(2) td:first-child").InnerHtml;

            secondProductNameAfterSorting.ShouldNotBe(initialProductName);
        }
}
}