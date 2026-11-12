using FluentValidation;
using NationalTeamManager.Application.CompetitionStage;

namespace NationalTeamManager.Application.Validators.CompetitionStage
{
    public class CompetitionStageQueryValidator : AbstractValidator<CompetitionStageQuery>
    {
        private static readonly string[] AllowedSortFields =
        [
            "name",
            "competitioneditionname",
            "competitionname",
        ];

        public CompetitionStageQueryValidator()
        {
            RuleFor(x => x.CompetitionEditionId)
                .GreaterThan(0)
                .When(x => x.CompetitionEditionId.HasValue);
            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Search));
            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage(
                    "Érvénytelen rendezési mező."
                        + "Lehetséges értékek: name, competitionEditionName, competitionName."
                );
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }

        private static bool BeValidSortField(string? sortBy) =>
            AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }
}
