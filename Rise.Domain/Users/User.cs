

using System.Security.Principal;

namespace Rise.Domain.Users
{
    

    public class User: Entity
    {
        private string voornaam = default!;
        private string naam = default!;
        private string email = default!;
        private string telNr = default!;
        private Rol rol = default!; 
        //wachtwoord en mogelijk lijst met producten moeten nog komen

        public required string Naam
        {
            get => naam;
            set => naam = Guard.Against.NullOrWhiteSpace(value);
        }

        public required string Voornaam
        {
            get => voornaam;
            set => voornaam = Guard.Against.NullOrWhiteSpace(value);
        }

        public required string Email
        {
            get => email;
            set => email = Guard.Against.NullOrWhiteSpace(value);
        }

        public required string TelNr
        {
            get => telNr;
            set => telNr = Guard.Against.NullOrWhiteSpace(value);
        }

        public required Rol Rol
        {
            get => rol;
            set => rol = value;
        }
    }
}
