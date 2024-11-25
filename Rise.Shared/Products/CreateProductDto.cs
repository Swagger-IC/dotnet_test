using FluentValidation;

namespace Rise.Shared.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reusable { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public int MinStock { get; set; }
        public string Keywords { get; set; } = string.Empty;
        public int LeverancierId { get; set; } 

        public class Validator : AbstractValidator<CreateProductDto>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Naam is verplicht.")
                    .MaximumLength(100).WithMessage("Naam mag maximaal 100 tekens lang zijn.");

                RuleFor(x => x.Location)
                    .NotEmpty().WithMessage("Locatie is verplicht.")
                    .MaximumLength(100).WithMessage("Locatie mag maximaal 100 tekens lang zijn.");

                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Beschrijving is verplicht.")
                    .MaximumLength(500).WithMessage("Beschrijving mag maximaal 500 tekens lang zijn.");

                RuleFor(x => x.Reusable)
                    .NotEmpty()
                    .WithMessage("Herbruikbaar moet 'Ja' of 'Nee' zijn.");

                RuleFor(x => x.Quantity)
                    .NotEmpty().WithMessage("Aantal is verplicht.")
                    .GreaterThan(0).WithMessage("Aantal moet een niet-negatief getal zijn en groter dan 0.");

                RuleFor(x => x.Barcode)
                    .NotEmpty().WithMessage("Barcode is verplicht.")
                    .MaximumLength(50).WithMessage("Barcode mag maximaal 50 tekens lang zijn.");

                //RuleFor(x => x.ImgUrl)
                //    .NotEmpty().WithMessage("Afbeeldings-URL is verplicht.");
                //.Must(BeAValidUrl).WithMessage("Afbeeldings-URL is ongeldig.");

                RuleFor(x => x.MinStock)
                    .NotEmpty().WithMessage("Minimale voorraad is verplicht")
                    .GreaterThan(0).WithMessage("Minimale voorraad moet een niet-negatief getal zijn en groter dan 0.");

                RuleFor(x => x.Keywords)
                    .NotEmpty().WithMessage("Trefwoorden is verplicht.")
                    .MaximumLength(200).WithMessage("Trefwoorden mogen maximaal 200 tekens lang zijn.");
            }

            
        }
    }
}
