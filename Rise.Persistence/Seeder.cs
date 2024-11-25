
using Rise.Domain.Users;
using Rise.Domain.Leveranciers;
using Rise.Domain.Products;

namespace Rise.Persistence;

public class Seeder
{
    private readonly ApplicationDbContext dbContext;

    public Seeder(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Seed()
    {

        if (HasAlreadyBeenSeeded())
            return;
        //this.dbContext.Database.EnsureDeleted();

        SeedLeveranciers();
        SeedProducts();
        SeedUsers();
    }

    private bool HasAlreadyBeenSeeded()
    {
        return dbContext.Products.Any() || dbContext.Leveranciers.Any() || dbContext.Users.Any();
    }

    private void SeedLeveranciers()
    {
        dbContext.Leveranciers.RemoveRange(dbContext.Leveranciers);

        var leveranciers = new List<Leverancier>
            {
                new Leverancier { Name = "Leverancier A", Email = "a@example.com", Address = "Address A" },
                new Leverancier { Name = "Leverancier B", Email = "b@example.com", Address = "Address B" },
                new Leverancier { Name = "Leverancier C", Email = "c@example.com", Address = "Address C" }
            };

        dbContext.Leveranciers.AddRange(leveranciers);
        dbContext.SaveChanges();
    }

    private void SeedUsers()
    {
        dbContext.Users.RemoveRange(dbContext.Users);

        var users = new List<User>
            {
                new User { Voornaam = "Michiel", Email = "michiel_murphy@outlook.com", Naam = "Murphy" },
                new User { Voornaam = "Corneel", Email = "corneel.verstraeten@student.hogent.be", Naam = "Verstraeten" },
                new User { Voornaam = "Denzell", Email = "denzell@gmail.com", Naam = "Boelens" }
            };
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }


    private void SeedProducts()
    {

        if (dbContext.Products.Any())
            return;

        var leveranciers = dbContext.Leveranciers.ToList();











        var products = new List<Product>
    {
        new Product
        {
            Name = "Microscoop",
            Location = "Labo 1.145",
            Description = "Hoge kwaliteit microscoop voor laboratoriumgebruik.",
            Reusable = true,
            Quantity = 5,
            Barcode = "123456001",
            MinStock = 2,
            Keywords = "microscoop,laboratorium",
            Leverancier = leveranciers[1],
            ImgUrl ="https://s3.360zorg.me/products/84150ae3-fa6e-434e-8e6b-4c9280ce3358.webp"
        },
        new Product
        {
            Name = "Digitale thermometer",
            Location = "Lokaal 2",
            Description = "Digitale thermometer voor nauwkeurige temperatuurmetingen.",
            Reusable = true,
            Quantity = 20,
            Barcode = "123456002",
            MinStock = 5,
            Keywords = "thermometer,temperatuur,medisch",
            Leverancier = leveranciers[1],
            ImgUrl ="https://s3.360zorg.me/products/0d3c9df4-efbe-454d-b2b7-4fc2c497674f.webp"
        },
        new Product
        {
            Name = "Bloeddrukmeter",
            Location = "Lokaal 2",
            Description = "Automatische bloeddrukmonitor.",
            Reusable = true,
            Quantity = 15,
            Barcode = "123456003",
            MinStock = 1,
            Keywords = "bloeddrukmeter,medisch",
            Leverancier = leveranciers[2],
            ImgUrl ="https://s3.360zorg.me/products/037926e0-f2bc-4249-9713-f059cc3f62ac.webp"
        },
        new Product
        {
            Name = "EHBO kit",
            Location = "Lokaal 3",
            Description = "Compleet EHBO-kist voor noodgevallen.",
            Reusable = false,
            Quantity = 10,
            Barcode = "123456004",
            MinStock = 10,
            Keywords = "ehbo,kit,medisch",
            Leverancier = leveranciers[0],
             ImgUrl ="https://s3.360zorg.me/products/96cabdb8-bdee-4b0a-867c-75c7e42ffd97.webp"
        },
        new Product
        {
            Name = "Pleister",
            Location = "Lokaal 3",
            Description = "Elastische bandage voor medisch gebruik.",
            Reusable = false,
            Quantity = 50,
            Barcode = "123456005",
            MinStock = 100,
            Keywords = "pleister,medisch,wondzorg",
            Leverancier = leveranciers[1],
            ImgUrl ="https://s3.360zorg.me/products/c073bcd7-8038-4e3b-bd37-7fc50ebbb2d4.webp"
        },
        new Product
        {
            Name = "Spuit",
            Location = "Lokaal 123",
            Description = "Wegwerpspuit voor injecties.",
            Reusable = false,
            Quantity = 100,
            Barcode = "123456006",
            MinStock = 20,
            Keywords = "spuit,medisch,labo",
            Leverancier = leveranciers[0],
            ImgUrl ="https://s3.360zorg.me/products/f0d0fc73-f516-4fe1-ae12-15d770741f16.webp"
        },
        new Product
        {
            Name = "Zuurstofmasker",
            Location = "Lokaal 123",
            Description = "Zuurstofmasker voor ademhalingsondersteuning.",
            Reusable = true,
            Quantity = 25,
            Barcode = "123456007",
            MinStock = 5,
            Keywords = "zuurstofmasker,ademhaling,medisch",
            Leverancier = leveranciers[2],
             ImgUrl ="https://s3.360zorg.me/products/4722493f-6885-4212-b963-c10052bb1630.webp"
        },
        new Product
        {
            Name = "Scalpel",
            Location = "Lokaal 6",
            Description = "Chirurgisch scalpel voor nauwkeurige incisie.",
            Reusable = false,
            Quantity = 20,
            Barcode = "123456008",
            MinStock = 5,
            Keywords = "scalpel,chrirurgie,medisch",
            Leverancier = leveranciers[2],
            ImgUrl ="https://s3.360zorg.me/products/e1a8df5f-2133-4678-8041-5d12ee9a3b43.webp"
        },
        new Product
        {
            Name = "Chirurgische handschoenen",
            Location = "Lokaal 8",
            Description = "Steriele chirurgische handschoenen voor medisch gebruik.",
            Reusable = false,
            Quantity = 200,
            Barcode = "123456009",
            MinStock = 20,
            Keywords = "handschoenen,chrirurgie,medisch",
            Leverancier = leveranciers[1],
             ImgUrl ="https://s3.360zorg.me/products/54e8d76f-04e4-4715-8de3-7d829073d119.webp"
        },
        new Product
        {
            Name = "Glucometer",
            Location = "Lokaal 9",
            Description = "Draagbare glucometer voor diabetesbeheer.",
            Reusable = false,
            Quantity = 12,
            Barcode = "123456010",
            MinStock = 2,
            Keywords = "glucosemeter,diabetes,gezondheid",
            Leverancier = leveranciers[2],
            ImgUrl ="https://s3.360zorg.me/products/b59a9252-342d-41b0-972f-87f001247fea.webp"
        },
        new Product
        {
            Name = "ECG-machine",
            Location = "Hartbewaking",
            Description = "ECG-apparaat voor hartmonitoring.",
            Reusable = true,
            Quantity = 3,
            Barcode = "123456011",
            MinStock = 1,
            Keywords = "ECG,hartmonitor,medisch",
            Leverancier = leveranciers[0],
            ImgUrl ="https://s3.360zorg.me/products/fd30879a-704b-43d7-9f74-49a76f67174d.webp"
        },
        new Product
        {
            Name = "Vernevelaar",
            Location = "Lokaal 1",
            Description = "Vernevelaar voor ademhalingsbehandelingen.",
            Reusable = false,
            Quantity = 8,
            Barcode = "123456012",
            MinStock = 10,
            Keywords = "vernevelaar,ademhaling,medisch",
            Leverancier = leveranciers[2],
             ImgUrl ="https://s3.360zorg.me/products/5f53f6fa-3e8b-4a7c-8540-558bc3738a55.webp"
        },
        new Product
        {
            Name = "Katheter",
            Location = "Lokaal 2",
            Description = "Steriele katheter voor medische procedures.",
            Reusable = false,
            Quantity = 30,
            Barcode = "123456013",
            MinStock = 5,
            Keywords = "katheter,medisch,chrirurgie",
            Leverancier = leveranciers[2],
            ImgUrl ="https://s3.360zorg.me/products/6302ee26-02c3-45d6-a66e-a618c4ab2798.webp"
        },
        new Product
        {
            Name = "Infusieset",
            Location = "Lokaal 1",
            Description = "Infusieset voor het toedienen van vloeistoffen.",
            Reusable = false,
            Quantity = 25,
            Barcode = "123456014",
            MinStock = 10,
            Keywords = "infusieset,toedienen,medisch",
            Leverancier = leveranciers[1],
               ImgUrl ="https://s3.360zorg.me/products/8254e1b2-e9b6-4c07-acf1-7bc9b4fb02e5.webp"
        },
        new Product
        {
            Name = "Rolstoel",
            Location = "Lokaal 13",
            Description = "Lichte rolstoel voor patiëntvervoer.",
            Reusable = true,
            Quantity = 5,
            Barcode = "123456015",
            MinStock = 2,
            Keywords = "rolstoel,mobiliteit,medisch",
            Leverancier = leveranciers[2],
            ImgUrl ="https://s3.360zorg.me/products/cb2c6667-a686-4d3b-a9ee-e41ba00ac08b.webp"
        },
        new Product
        {
            Name = "Stethoscoop",
            Location = "Lokaal 2",
            Description = "Hoge kwaliteit stethoscoop voor auscultatie.",
            Reusable = true,
            Quantity = 10,
            MinStock = 1,
            Barcode = "123456016",
            Keywords = "stethoscoop,auscultatie,medisch",
            Leverancier = leveranciers[0],
             ImgUrl ="https://s3.360zorg.me/products/cf4278fe-f04b-4146-86ff-e88afc8837e9.webp"
        },
        new Product
        {
            Name = "Puls Oximeter",
            Location = "Lokaal 2",
            Description = "Apparaat voor het meten van zuurstofniveaus in het bloed.",
            Reusable = true,
            Quantity = 15,
            Barcode = "123456017",
            MinStock = 1,
            Keywords = "puls oximeter,zuurstof,medisch",
            Leverancier = leveranciers[2],
            ImgUrl ="https://s3.360zorg.me/products/f2a69581-84ed-4d2b-9e41-c1258d1750a7.webp"
        },
        new Product
        {
            Name = "Draagbare Röntgenmachine",
            Location = "Radiologie Kamer",
            Description = "Draagbare machine voor het maken van röntgenfoto's.",
            Reusable = true,
            Quantity = 2,
            Barcode = "123456018",
            MinStock = 1,
            Keywords = "röntgen,draagbaar,radiologie",
            Leverancier = leveranciers[1],
             ImgUrl ="https://s3.360zorg.me/products/2017ca2f-1bba-4756-be29-07ffcaec31f4.webp"
        },
        new Product
        {
            Name = "Hechtingen",
            Location = "Lokaal 16",
            Description = "Verscheidenheid aan hechtingen voor wondverzorging.",
            Reusable = false,
            Quantity = 100,
            Barcode = "123456019",
            MinStock = 20,
            Keywords = "hechtingen,wondzorg,medisch",
            Leverancier = leveranciers[0],
            ImgUrl ="https://s3.360zorg.me/products/adad5108-a0df-4644-a96e-96f17c7d709e.webp"
        },
        new Product
        {
            Name = "Chirurgische tape",
            Location = "Lokaal 1",
            Description = "Medische tape voor het vastzetten van verbanden.",
            Reusable = false,
            Quantity = 50,
            Barcode = "123456020",
            MinStock = 15,
            Keywords = "chirurgische tape,verband,medisch",
            Leverancier = leveranciers[2],
             ImgUrl ="https://s3.360zorg.me/products/12f7b7d2-ebc9-46cd-971e-2ead0db81094.webp"
        },
        new Product
        {
            Name = "Petri schalen",
            Location = "Labo 1.146",
            Description = "Petri schalen voor microbiologische culturen.",
            Reusable = false,
            Quantity = 200,
            Barcode = "123456021",
            MinStock = 50,
            Keywords = "petri schaal,microbiologie,laboratorium",
            Leverancier = leveranciers[0],
             ImgUrl ="https://s3.360zorg.me/products/2d6a9725-c05a-4781-9b9e-dc675a082ac0.webp"
        },

        new Product
        {
            Name = "Micropipetten",
            Location = "Labo 1.150",
            Description = "Micropipetten voor nauwkeurige vloeistofverplaatsing.",
            Reusable = true,
            Quantity = 20,
            MinStock = 20,
            Barcode = "123456025",
            Keywords = "micropipet,laboratorium,biologie",
            Leverancier = leveranciers[2],
             ImgUrl ="https://s3.360zorg.me/products/ec488832-6b07-4002-a060-12addcab1992.webp"
        },

    };


        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }
}