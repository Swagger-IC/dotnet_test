using Microsoft.AspNetCore.Components;

namespace Rise.Client.Users.Components
{
    public partial class PersoonlijkInfoKaart
    {
        [Parameter] public required string Voornaam { get; set; }
        [Parameter] public required string Naam { get; set; }
        [Parameter] public required string Email { get; set; }
    }
}