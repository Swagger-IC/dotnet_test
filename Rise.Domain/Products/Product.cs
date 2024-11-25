using Rise.Domain.Leveranciers;
namespace Rise.Domain.Products;
public class Product : Entity
{
    private string name = default!;
    private string location = default!;
    private string description = default!;
    private bool reusable = default!;
    private int quantity = default!;
    private string barcode = default!;
    private int minStock = default!;
    private string keywords = default!;
    private Leverancier leverancier = default!;
    private string imgUrl = default!;

    public required string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value);
    }

    public required string Location
    {
        get => location;
        set => location = Guard.Against.NullOrWhiteSpace(value);
    }

    public required string Description
    {
        get => description;
        set => description = Guard.Against.NullOrWhiteSpace(value);
    }

    public required bool Reusable
    {
        get => reusable;
        set => reusable = value;
    }

    public required int Quantity
    {
        get => quantity;
        set => quantity = Guard.Against.Negative(value);
    }

    public required string Barcode
    {
        get => barcode;
        set => barcode = Guard.Against.NullOrWhiteSpace(value);
    }

    public required int MinStock
    {
        get => minStock;
        set => minStock = Guard.Against.Negative(value);
    }

    public required string Keywords
    {
        get => keywords;
        set => keywords = Guard.Against.NullOrWhiteSpace(value);
    }

    public required Leverancier Leverancier
    {
        get => leverancier;
        set
        {
            leverancier = Guard.Against.Null(value);
            leverancier.AddProduct(this);
        }
    }

    public required string ImgUrl
    {
        get => imgUrl;
        set => imgUrl = Guard.Against.NullOrWhiteSpace(value);
    }
}