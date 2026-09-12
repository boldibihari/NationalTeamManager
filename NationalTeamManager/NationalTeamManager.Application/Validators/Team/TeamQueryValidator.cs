using FluentValidation;
using NationalTeamManager.Application.Team;

namespace NationalTeamManager.Application.Validators.Team
{
    public class TeamQueryValidator : AbstractValidator<TeamQuery>
    {
        private static readonly string[] AllowedSortFields = ["name", "country"];

        public TeamQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.Country)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Country));

            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage("Érvénytelen rendezési mező. " + "Lehetséges értékek: name, country.");

            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }

        private static bool BeValidSortField(string? sortBy)
        {
            return AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
        }
    }
}
