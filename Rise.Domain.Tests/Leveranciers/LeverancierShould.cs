using Rise.Domain.Leveranciers;
using Rise.Domain.Products;
using Shouldly;

namespace Rise.Domain.Tests.Leveranciers;

public class LeverancierShould
{
    [Fact]
    public void BeCreatedWithValidProperties()
    {
        var leverancier = new Leverancier
        {
            Name = "Medical Supplies Inc.",
            Email = "info@medsupplies.com",
            Address = "123 Health Street"
        };

        leverancier.Name.ShouldBe("Medical Supplies Inc.");
        leverancier.Email.ShouldBe("info@medsupplies.com");
        leverancier.Address.ShouldBe("123 Health Street");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreatedWithInvalidName(string? invalidName)
    {
        Action act = () => new Leverancier
        {
            Name = invalidName!, 
            Email = "info@medsupplies.com",
            Address = "123 Health Street"
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreatedWithInvalidEmail(string? invalidEmail)
    {
        Action act = () => new Leverancier
        {
            Name = "Medical Supplies Inc.",
            Email = invalidEmail!, 
            Address = "123 Health Street"
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NotBeCreatedWithInvalidAddress(string? invalidAddress)
    {
        
        Action act = () => new Leverancier
        {
            Name = "Medical Supplies Inc.",
            Email = "info@medsupplies.com",
            Address = invalidAddress!  
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void AllowProductToBeAdded()
    {
        var leverancier = new Leverancier
        {
          Name = "Medical Supplies Inc.",
          Email = "info@medsupplies.com",
          Address = "123 Health Street"
        };

        var product = new Product
        {
            Name = "Surgical Mask",
            Location = "Medical Supply Room",
            Description = "A disposable surgical mask to protect against airborne pathogens.",
            Reusable = false,
            Quantity = 100,
            Barcode = "123456789",
            Keywords = "mask, protection",
            MinStock = 5,
            Leverancier = leverancier,  // Dit voegt automatisch product toe aan leverancier, geen nood om leverancier.addProduct aan te roepen
            ImgUrl = "test.jpg"  
        };

        leverancier.Products.Count.ShouldBe(1);
        leverancier.Products[0].ShouldBe(product);
    }

}
