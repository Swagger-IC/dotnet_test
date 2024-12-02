using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Domain.Products;
using Rise.Domain.Leveranciers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using Rise.Domain.Orders;
using Rise.Domain.Users;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // In-memory database toevoegen
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Vervang authenticatie met een test scheme
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.EnsureCreated();
                SeedDatabase(dbContext);
            }
        });
    }

    //In memory test db seeden
    private void SeedDatabase(ApplicationDbContext dbContext)
    {
    
        var leverancier = new Leverancier
        {
            Name = "Supplier A",
            Email = "supplier@example.com",
            Address = "123 Supplier St."
        };

        // Herbruikbare producten:
        var product1 = new Product
        {
            Name = "Reusable Product 1",
            Location = "Location A",
            Description = "Test reusable product 1",
            Reusable = true,
            Quantity = 10,
            Barcode = "1234567890",
            MinStock = 5,
            Keywords = "reusable, A1",
            Leverancier = leverancier,
            ImgUrl = "test.jpg"
        };

        var product2 = new Product
        {
            Name = "Reusable Product 2",
            Location = "Location B",
            Description = "Test reusable product 2",
            Reusable = true,
            Quantity = 20,
            Barcode = "0987654321",
            MinStock = 5,
            Keywords = "reusable, B2",
            Leverancier = leverancier,
            ImgUrl = "test.jpg"
        };

        // Niet herbruikbare producten
        var product3 = new Product
        {
            Name = "Non-Reusable Product 1",
            Location = "Location C",
            Description = "Test non-reusable product 1",
            Reusable = false,
            Quantity = 5,
            Barcode = "5678901234",
            MinStock = 10,
            Keywords = "nonreusable, C1",
            Leverancier = leverancier,
            ImgUrl = "test.jpg"
        };

        var product4 = new Product
        {
            Name = "Non-Reusable Product 2",
            Location = "Location D",
            Description = "Test non-reusable product 2",
            Reusable = false,
            Quantity = 15,
            Barcode = "4321098765",
            MinStock = 5,
            Keywords = "nonreusable, D2",
            Leverancier = leverancier,
            ImgUrl = "test.jpg"
        };

        dbContext.Leveranciers.Add(leverancier);
        dbContext.Products.AddRange(product1, product2, product3, product4);
        // Voeg een test-gebruiker toe
        var user = new User
        {
            Voornaam = "Pol",
            Naam = "test",
            Email = "test@example.com",
        };

        dbContext.Users.Add(user);

        // Maak orders en gebruik AddOrderItem
        var order1 = new Order
        {
            User = user
        };
        order1.AddOrderItem(product1, 2);
        order1.AddOrderItem(product3, 1);

        var order2 = new Order
        {
            User = user
        };
        order2.AddOrderItem(product2, 5);
        order2.AddOrderItem(product4, 3);

        dbContext.Orders.AddRange(order1, order2);

        // Sla wijzigingen op in de in-memory database
        dbContext.SaveChanges();
    }
}
