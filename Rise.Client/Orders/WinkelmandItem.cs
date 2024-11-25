namespace Rise.Client.Orders;

public class WinkelmandItem
{
    public int ProductId { get; init; }
     public string Name { get; init; }
     public int Amount { get; set; }
     public int InStock {get; set;}
     public WinkelmandItem(int productId, string name, int amount, int inStock)
     {
         ProductId = productId;
         Name = name;
         Amount = amount;
         InStock = inStock;
     }
}
