/*using Rise.Shared.Products;
using Xunit.Abstractions;
using Shouldly;
using NSubstitute;
using System.Threading.Tasks;
using System.Linq;

namespace Rise.Client.Products;

/// <summary>
/// Same as <see cref="IndexShould"/> using mocking instead of faking.
/// https://nsubstitute.github.io
/// </summary>
public class IndexWithMockExampleShould : TestContext
{
    public IndexWithMockExampleShould(ITestOutputHelper outputHelper)
    {
        Services.AddXunitLogger(outputHelper);
    }

    [Fact]
    public void ShowsProducts()
    {
        var products = Enumerable.Range(1, 5)
                         .Select(i => new ProductDto { Id = i, Name = $"Product {i}" });

        var productServiceMock = Substitute.For<IProductService>();
        productServiceMock.GetProductsAsync().Returns(Task.FromResult(products));

        Services.AddScoped(provider => productServiceMock);

        var cut = RenderComponent<Index>();
        cut.FindAll("table tbody tr").Count.ShouldBe(5);
    }
}*/