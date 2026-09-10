using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class CompetitionStage : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CompetitionEditionId { get; set; }
        public CompetitionEdition CompetitionEdition { get; set; } = null!;
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
        public ICollection<Match> Matches { get; set; } = [];
    }
}
