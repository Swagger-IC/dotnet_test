using FluentValidation;

namespace Rise.Shared.Users
{
    public class CreateUserDto
    {

        public string Email { get; set; } = string.Empty; //string.Empty staat hier om warnings in verband met nullable weg te halen
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string RoleId { get; set; } = string.Empty;


        public class Validator : AbstractValidator<CreateUserDto>
        {
            public Validator()
            {

                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Voornaam is verplicht.")
                    .MaximumLength(100).WithMessage("Voornaam mag maximaal 100 tekens lang zijn.");

                RuleFor(x => x.Email)
                     .NotEmpty().WithMessage("Email is verplicht.")
                     .MaximumLength(500).WithMessage("Email mag maximaal 500 tekens lang zijn.")
                     .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$").WithMessage("Email moet een geldig e-mailadres zijn.");


                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Naam is verplicht.")
                    .MaximumLength(100).WithMessage("Naam mag maximaal 100 tekens lang zijn.");

                RuleFor(x => x.Password)
                     .NotEmpty().WithMessage("Wachtwoord is verplicht.")
                     .MinimumLength(8).WithMessage("Wachtwoord moet minimaal 8 tekens lang zijn.")
                     .Matches("[A-Z]").WithMessage("Wachtwoord moet minimaal één hoofdletter bevatten.")
                     .Matches("[a-z]").WithMessage("Wachtwoord moet minimaal één kleine letter bevatten.")
                     .Matches("[0-9]").WithMessage("Wachtwoord moet minimaal één cijfer bevatten.")
                     .Matches("[^a-zA-Z0-9]").WithMessage("Wachtwoord moet minimaal één speciaal teken bevatten.");

                RuleFor(x => x.RoleId)
                    .NotEmpty().WithMessage("Een rol is verplicht.");


            }

        }
    }
}
