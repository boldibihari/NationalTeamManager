using FluentValidation;
using NationalTeamManager.Application.Competition;

namespace NationalTeamManager.Application.Validators.Competition
{
    public class CompetitionQueryValidator : AbstractValidator<CompetitionQuery>
    {
        private static readonly string[] AllowedSortFields = ["name", "country"];

        public CompetitionQueryValidator()
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
                .WithMessage("Érvénytelen rendezési mező." + "Lehetséges értékek: name, country.");
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }

        private static bool BeValidSortField(string? sortBy) =>
            AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }
}
