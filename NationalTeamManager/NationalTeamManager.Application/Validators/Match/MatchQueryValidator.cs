using FluentValidation;
using NationalTeamManager.Application.Match;

namespace NationalTeamManager.Application.Validators.Match
{
    public class MatchQueryValidator : AbstractValidator<MatchQuery>
    {
        private static readonly string[] AllowedSortFields = ["date"];

        public MatchQueryValidator()
        {
            RuleFor(x => x.From)
                .LessThanOrEqualTo(x => x.To)
                .When(x => x.From.HasValue && x.To.HasValue)
                .WithMessage("A kezdő dátum nem lehet későbbi, mint a záró dátum.");

            RuleFor(x => x.TeamId).GreaterThan(0).When(x => x.TeamId.HasValue);

            RuleFor(x => x.CompetitionEditionId)
                .GreaterThan(0)
                .When(x => x.CompetitionEditionId.HasValue);

            RuleFor(x => x.CompetitionStageId)
                .GreaterThan(0)
                .When(x => x.CompetitionStageId.HasValue);

            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage("Érvénytelen rendezési mező. " + "Lehetséges érték: date.");

            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }

        private static bool BeValidSortField(string? sortBy)
        {
            return AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
        }
    }
}
