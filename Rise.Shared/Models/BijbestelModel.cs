using System.ComponentModel.DataAnnotations;

namespace Rise.Shared.Models;
public class BijbestelModel
{
    [Required(ErrorMessage = "Hoeveelheid is verplicht")]
    [Range(1, int.MaxValue, ErrorMessage = "Hoeveelheid moet groter zijn dan 0")]
    public int? QuantityToAdd { get; set; }

    public string? ServerError { get; set; }
}