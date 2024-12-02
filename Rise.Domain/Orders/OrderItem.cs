using Ardalis.GuardClauses;
using Rise.Domain.Products;

namespace Rise.Domain.Orders
{
    public class OrderItem : Entity
    {
        private Product product = default!;
        private int amount = default!;

        public required Product Product
        {
            get => product;
            set => product = Guard.Against.Null(value);
        }

        public int Amount
        {
            get => amount;
            set => amount = Guard.Against.NegativeOrZero(value); // Orders should have at least 1 item
        }
    }
}