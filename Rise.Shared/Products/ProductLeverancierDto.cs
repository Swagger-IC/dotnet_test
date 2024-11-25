namespace Rise.Shared.Products;

public class ProductLeverancierDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Description { get; set; }
    public required bool Reusable { get; set; }
    public required int Quantity { get; set; }
    public required string Barcode { get; set; }
    public required int MinStock {get; set;}

    public required string Keywords { get; set; }
    public required string LeverancierEmail {get; set;}
}