using FluentValidation;
using NationalTeamManager.Application.Players;
using NationalTeamManager.Domain.Enums;

namespace NationalTeamManager.Application.Validators.Player
{
    public class PlayerQueryValidator : AbstractValidator<PlayerQuery>
    {
        private static readonly string[] AllowedSortFields = ["name", "marketvalue", "height"];

        public PlayerQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.Position)
                .Must(BeValidPosition)
                .When(x => !string.IsNullOrWhiteSpace(x.Position))
                .WithMessage("Érvénytelen pozíció.");

            RuleFor(x => x.TeamId).GreaterThan(0).When(x => x.TeamId.HasValue);

            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage(
                    "Érvénytelen rendezési mező. "
                        + "Lehetséges értékek: name, marketValue, height."
                );

            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }

        private static bool BeValidPosition(string? position)
        {
            return Enum.TryParse<PlayerPosition>(position, true, out _);
        }

        private static bool BeValidSortField(string? sortBy)
        {
            return AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
        }
    }
}
