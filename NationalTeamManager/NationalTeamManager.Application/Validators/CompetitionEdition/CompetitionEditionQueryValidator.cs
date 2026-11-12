using FluentValidation;
using NationalTeamManager.Application.CompetitionEdition;

namespace NationalTeamManager.Application.Validators.CompetitionEdition
{
    public class CompetitionEditionQueryValidator : AbstractValidator<CompetitionEditionQuery>
    {
        private static readonly string[] AllowedSortFields = ["name", "year", "competitionname"];

        public CompetitionEditionQueryValidator()
        {
            RuleFor(x => x.CompetitionId).GreaterThan(0).When(x => x.CompetitionId.HasValue);
            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Search));
            RuleFor(x => x.Year).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Year));
            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage(
                    "Érvénytelen rendezési mező."
                        + "Lehetséges értékek: name, year, competitionName."
                );
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }

        private static bool BeValidSortField(string? sortBy) =>
            AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }
}
