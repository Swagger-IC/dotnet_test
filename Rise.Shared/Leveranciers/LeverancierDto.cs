using System.Text.Json.Serialization;

namespace Rise.Shared.Leveranciers
{
    public class LeverancierDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }

        //public private List<Product> products { get; }
    }
}