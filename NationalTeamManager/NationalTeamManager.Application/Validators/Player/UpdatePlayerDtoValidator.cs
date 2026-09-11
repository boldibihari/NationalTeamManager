using FluentValidation;
using NationalTeamManager.Application.Players.Dtos;

namespace NationalTeamManager.Application.Validators.Player
{
    public class UpdatePlayerDtoValidator : AbstractValidator<UpdatePlayerDto>
    {
        public UpdatePlayerDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Position).IsInEnum();

            RuleFor(x => x.Height).GreaterThan(0).When(x => x.Height.HasValue);

            RuleFor(x => x.DateOfBirth)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.PreferredFoot).IsInEnum();

            RuleFor(x => x.Nationality)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Nationality));

            RuleFor(x => x.TeamId).GreaterThan(0).When(x => x.TeamId.HasValue);
        }
    }
}
