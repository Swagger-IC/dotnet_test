﻿using Rise.Domain.Products;

namespace Rise.Domain.Leveranciers
{
    public class Leverancier : Entity
    {
        private string name = default!;
        private string email = default!;
        private string address = default!;
        private List<Product> products = new List<Product>();

        public required string Name
        {
            get => name;
            set => name = Guard.Against.NullOrWhiteSpace(value);
        }

        public required string Email
        {
            get => email;
            set => email = Guard.Against.NullOrWhiteSpace(value);
        }

        public required string Address
        {
            get => address;
            set => address = Guard.Against.NullOrWhiteSpace(value);
        }

        public IReadOnlyList<Product> Products => products.AsReadOnly(); 

        public void AddProduct(Product product)
        {
            products.Add(product);
        }
    }
}