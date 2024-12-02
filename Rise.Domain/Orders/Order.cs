using Ardalis.GuardClauses;
using Rise.Domain.Products;
using Rise.Domain.Users;
using System.Collections.Generic;

namespace Rise.Domain.Orders
{
    public class Order : Entity
    {
        private User user = default!;
        private List<OrderItem> orderItems = new();

        public User User
        {
            get => user;
            set => user = Guard.Against.Null(value);
        }

        public List<OrderItem> OrderItems
        {
            get => orderItems;
            set => orderItems = value ?? new List<OrderItem>();
        }

        public void AddOrderItem(Product product, int amount)
        {
            Guard.Against.Null(product, nameof(product));
            Guard.Against.NegativeOrZero(amount, nameof(amount));

            var existingItem = orderItems.FirstOrDefault(item => item.Product == product);
            if (existingItem is not null)
            {
                existingItem.Amount += amount;
            }
            else
            {
                orderItems.Add(new OrderItem
                {
                    Product = product,
                    Amount = amount
                });
            }
        }
    }
}
