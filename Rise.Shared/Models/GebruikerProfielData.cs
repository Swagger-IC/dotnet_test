using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Models
{
    public class Gebruiker
    {
        public required int Id { get; set; }
        public required string Naam { get; set; }
        public required string Email { get; set; }
        public required string TelNr { get; set; }

    }

    public class Product
    {
        public required int Id { get; set; }
        public required string Naam { get; set; }
        public required int AantalInStock { get; set; }
    }
}
