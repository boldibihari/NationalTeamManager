namespace NationalTeamManager.Domain.Entities
{
    public class CompetitionEdition
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Year { get; set; }

        public int CompetitionId { get; set; }

        public Competition Competition { get; set; } = null!;

        public string? ExternalId { get; set; }

        public string? DataSource { get; set; }

        public ICollection<CompetitionStage> Stages { get; set; } = [];

        public ICollection<Match> Matches { get; set; } = [];
    }
}
