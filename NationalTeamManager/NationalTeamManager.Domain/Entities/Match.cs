using NationalTeamManager.Domain.Interfaces;

namespace NationalTeamManager.Domain.Entities
{
    public class Match : IEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? Venue { get; set; }
        public int CompetitionEditionId { get; set; }
        public CompetitionEdition CompetitionEdition { get; set; } = null!;
        public int? CompetitionStageId { get; set; }
        public CompetitionStage? CompetitionStage { get; set; }
        public string? ExternalId { get; set; }
        public string? DataSource { get; set; }
    }
}
